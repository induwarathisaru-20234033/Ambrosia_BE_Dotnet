using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Infra.Repositories
{
    public class PurchaseRequestRepository : IPurchaseRequestRepositoy
    {
        private readonly AMBContext _context;

        public PurchaseRequestRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<PurchaseRequest> AddAsync(PurchaseRequest purchaseRequest)
        {
            _context.PurchaseRequests.Add(purchaseRequest);
            await _context.SaveChangesAsync();
            return purchaseRequest;
        }

        public async Task<PurchaseRequest?> GetByIdAsync(int id)
        {
            return await _context.PurchaseRequests
                .Include(pr => pr.PRItems)
                .ThenInclude(pri => pri.InventoryItem)
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<PurchaseRequest?> UpdateAsync(PurchaseRequest purchaseRequest)
        {
            var existing = await _context.PurchaseRequests
                .Include(pr => pr.PRItems)
                .FirstOrDefaultAsync(pr => pr.Id == purchaseRequest.Id);

            if (existing == null)
            {
                return null;
            }

            existing.Description = purchaseRequest.Description;
            existing.Supplier = purchaseRequest.Supplier;
            existing.RequestedBy = purchaseRequest.RequestedBy;
            existing.RequestedDeliveryDate = purchaseRequest.RequestedDeliveryDate;
            existing.IsUrgent = purchaseRequest.IsUrgent;
            existing.PurchaseRequestStatus = purchaseRequest.PurchaseRequestStatus;
            existing.Status = purchaseRequest.Status;
            existing.ReviewedBy = purchaseRequest.ReviewedBy;
            existing.ReviewedDate = purchaseRequest.ReviewedDate;

            _context.PurchaseRequestItems.RemoveRange(existing.PRItems);
            existing.PRItems.Clear();

            foreach (var item in purchaseRequest.PRItems)
            {
                existing.PRItems.Add(new PurchaseRequestItem
                {
                    LineItemNo = item.LineItemNo,
                    RequestedQuantity = item.RequestedQuantity,
                    Price = item.Price,
                    InventoryItemId = item.InventoryItemId,
                    Status = item.Status,
                });
            }

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> ExistsByCodeAsync(string purchaseRequestCode)
        {
            return await _context.PurchaseRequests
                .AsNoTracking()
                .AnyAsync(pr => pr.PurchaseRequestCode == purchaseRequestCode);
        }

        public IQueryable<PurchaseRequest> GetSearchQuery()
        {
            return _context.PurchaseRequests
                .Include(pr => pr.PRItems)
                .ThenInclude(pri => pri.InventoryItem)
                .AsNoTracking();
        }

        public async Task<Dictionary<string, string>> GetCreatorNamesByUsernamesAsync(List<string> creatorUsernames)
        {
            if (creatorUsernames == null || creatorUsernames.Count == 0)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var identifiers = creatorUsernames
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (identifiers.Count == 0)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var employees = await _context.Employees
                .AsNoTracking()
                .Where(employee => identifiers.Contains(employee.Username)
                    || (employee.Email != null && identifiers.Contains(employee.Email)))
                .Select(employee => new
                {
                    employee.Username,
                    employee.Email,
                    employee.FirstName,
                    employee.LastName,
                })
                .ToListAsync();

            var creatorNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var employee in employees)
            {
                var fullName = string.Join(" ", new[] { employee.FirstName, employee.LastName }
                    .Where(part => !string.IsNullOrWhiteSpace(part))).Trim();

                var displayName = string.IsNullOrWhiteSpace(fullName)
                    ? employee.Username
                    : fullName;

                if (!string.IsNullOrWhiteSpace(employee.Username) && !creatorNames.ContainsKey(employee.Username))
                {
                    creatorNames[employee.Username] = displayName;
                }

                if (!string.IsNullOrWhiteSpace(employee.Email) && !creatorNames.ContainsKey(employee.Email))
                {
                    creatorNames[employee.Email] = displayName;
                }
            }

            return creatorNames;
        }
    
    }
}
