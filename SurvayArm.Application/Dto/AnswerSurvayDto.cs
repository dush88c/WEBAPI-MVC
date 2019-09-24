using System;
using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public partial class AnswerSurvayDto 
    {
        public AnswerSurvayDto()
        {
            AnswerFields = new List<AnswerFieldDto>();
        }

        public int Id { get; set; }
        public int SurvayId { get; set; }   
        public int SurvayTypeID { get; set; }
        public int ClientId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DeviceId { get; set; }
        public string DeviceUniqueId { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public  List<AnswerFieldDto> AnswerFields { get; set; }        
        public  SurvayTypeDto SurvayType { get; set; }
        public  SurvayDto Survay { get; set; }
        public  ClientDto Client { get; set; }
        public  DeviceManagerDto DeviceManager { get; set; }
    }
}
