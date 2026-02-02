using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Dtos
{
    public class BaseResponseDto<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public T? Data { get; set; }


        public BaseResponseDto()
        {
        }

        public BaseResponseDto(T data, string message)
        {
            Succeeded = true;
            Message = message ?? "Request successful.";
            Data = data;
            Errors = [];
        }

        public BaseResponseDto(string message, List<string> errors)
        {
            Succeeded = false;
            Message = message;
            Errors = errors;
            Data = default;
        }
    }
}
