using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class ClientDto 
    {        
        public ClientDto()
        {
            AnswerSurvays = new List<AnswerSurvayDto>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthOfDate { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
       
        [JsonIgnore]
        public List<AnswerSurvayDto> AnswerSurvays { get; set; }
    }
}
