using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AMB.Application.Services
{
    public class WastageRecordService : IWastageRecordService
    {
        private readonly IWastageRecordRepository _wastageRecordRepository;

        public WastageRecordService(IWastageRecordRepository wastageRecordRepository)
        {
            _wastageRecordRepository = wastageRecordRepository;
        }

        public async Task<WastageRecordDto> CreateWastageRecordAsync(CreateWastageRecordDto request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("At least one wastage entry item is required.");

            var wastageRecord = new WastageRecord
            {
                WastageEntryNumber = GenerateUniqueWastageEntryNumber(),
                EntryDate = request.EntryDate,
                RecordedBy = request.RecordedBy,
                GeneralNotes = request.GeneralNotes,
                WastageEntryItems = request.Items.Select(item => new WastageEntryItem
                {
                    ItemNo = item.ItemNo,
                    WastageType = item.WastageType,
                    Quantity = item.Quantity,
                    Reason = item.Reason,
                    InventoryItemId = item.InventoryItemId
                }).ToList()
            };

            var result = await _wastageRecordRepository.AddAsync(wastageRecord);

            return new WastageRecordDto
            {
                Id = result.Id,
                WastageEntryNumber = result.WastageEntryNumber,
                EntryDate = result.EntryDate,
                RecordedBy = result.RecordedBy,
                GeneralNotes = result.GeneralNotes,
                Items = result.WastageEntryItems.Select(e => new WastageEntryItemDto
                {
                    Id = e.Id,
                    ItemNo = e.ItemNo,
                    WastageType = e.WastageType,
                    Quantity = e.Quantity,
                    Reason = e.Reason,
                    InventoryItemId = e.InventoryItemId,
                    InventoryItemName = e.InventoryItem?.ItemName ?? string.Empty
                }).ToList()
            };
        }

        public async Task<WastageRecordDto> UpdateWastageRecordAsync(UpdateWastageRecordDto request)
        {
            var existing = await _wastageRecordRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Wastage record with ID {request.Id} not found.");
            }
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one wastage entry item is required.");
            }
            var wastageRecord = new WastageRecord
            {
                Id = request.Id,
                WastageEntryNumber = existing.WastageEntryNumber,
                EntryDate = request.EntryDate,
                RecordedBy = request.RecordedBy,
                GeneralNotes = request.GeneralNotes,
                WastageEntryItems = request.Items.Select(item => new WastageEntryItem
                {
                    ItemNo = item.ItemNo,
                    WastageType = item.WastageType,
                    Quantity = item.Quantity,
                    Reason = item.Reason,
                    InventoryItemId = item.InventoryItemId
                }).ToList()
            };
            var result = await _wastageRecordRepository.UpdateAsync(wastageRecord);
            if (result == null)
            {
                throw new KeyNotFoundException($"Wastage record with ID {request.Id} not found after update.");
            }
            return new WastageRecordDto
            {
                Id = result.Id,
                WastageEntryNumber = result.WastageEntryNumber,
                EntryDate = result.EntryDate,
                RecordedBy = result.RecordedBy,
                GeneralNotes = result.GeneralNotes,
                Items = result.WastageEntryItems.Select(e => new WastageEntryItemDto
                {
                    Id = e.Id,
                    ItemNo = e.ItemNo,
                    WastageType = e.WastageType,
                    Quantity = e.Quantity,
                    Reason = e.Reason,
                    InventoryItemId = e.InventoryItemId,
                    InventoryItemName = e.InventoryItem?.ItemName ?? string.Empty
                }).ToList()
            };
        }

        private string GenerateUniqueWastageEntryNumber()
        {
            return $"WSTG-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public async Task<IEnumerable<WastageRecordDto>> GetAllWastageRecordsAsync()
        {
            var records = await _wastageRecordRepository.GetAllAsync();
            return records.Select(MapToDto);
        }

        public async Task<WastageRecordDto> GetWastageRecordByIdAsync(int id)
        {
            var record = await _wastageRecordRepository.GetByIdAsync(id);
            if (record == null)
                throw new KeyNotFoundException($"Wastage record with ID {id} not found.");

            return MapToDto(record);
        }
        private static WastageRecordDto MapToDto(WastageRecord record)
        {
            return new WastageRecordDto
            {
                Id = record.Id,
                WastageEntryNumber = record.WastageEntryNumber,
                EntryDate = record.EntryDate,
                RecordedBy = record.RecordedBy,
                GeneralNotes = record.GeneralNotes,
                Items = record.WastageEntryItems.Select(e => new WastageEntryItemDto
                {
                    Id = e.Id,
                    ItemNo = e.ItemNo,
                    WastageType = e.WastageType,
                    Quantity = e.Quantity,
                    Reason = e.Reason,
                    InventoryItemId = e.InventoryItemId,
                    InventoryItemName = e.InventoryItem?.ItemName ?? string.Empty
                }).ToList()
            };
        }
    }
}
