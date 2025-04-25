using SGBLApp.Core.Application.DTOs.Loans;
using SGBLApp.Core.Application.DTOs.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBLApp.Core.Application.DTOs.User
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<LoanDto> ActiveLoans { get; set; }
        public IEnumerable<ReservationDto> ActiveReservations { get; set; }
    }
}
