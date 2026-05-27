using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImportMasterWolf.Models.Table.WolfApproveCore_thaistanley
{


    [Table("MSTATACCEmployee")]
    public class ViewMSTATACCEmployee
    {
        [Key]
        public int EMPID { get; set; }
        public string EMPCODE { get; set; }
        public string Name { get; set; }
        public string NameTH { get; set; }
        public string JOB_NAME { get; set; }
        public string PositionName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string SECName { get; set; }
        public string GRPName { get; set; }
        public string UNTName { get; set; }
        public string DIRECT_INDIRECT_CODE { get; set; }
        public string INTERCOMNO { get; set; }
        public string NICKNAME { get; set; }
        //public string EMP_HEADCODE { get; set; }//report to employee

    }
    public class ViewCloneMSTATACCEmployee
    {
        [Key]
        public int EMPID { get; set; }
        public string EMPCODE { get; set; }
        public string Name { get; set; }
        public string NameTH { get; set; }
        public string JOB_NAME { get; set; }
        public string PositionName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string SECName { get; set; }
        public string GRPName { get; set; }
        public string UNTName { get; set; }
        public string DIRECT_INDIRECT_CODE { get; set; }
        public string INTERCOMNO { get; set; }
        public string NICKNAME { get; set; }
        public string DataStatus { get; set; } //for status
    }


    [Table("MSTATEmployee")]
    public class ViewMSTATEmployee
    {
        [Key]
        public int EMPID { get; set; }
        public string EMPCODE { get; set; }
        public string NICKNAME { get; set; }
        public string INTERCOMNO { get; set; }
        public string JOBCODE { get; set; }
        public string SECNAME { get; set; }
        public string GRPNAME { get; set; }
        public string UNTNAME { get; set; }
    }

    public class ViewCloneMSTATEmployee
    {
        [Key]
        public int EMPID { get; set; }
        public string EMPCODE { get; set; }
        public string NICKNAME { get; set; }
        public string INTERCOMNO { get; set; }
        public string JOBCODE { get; set; }
        public string SECNAME { get; set; }
        public string GRPNAME { get; set; }
        public string UNTNAME { get; set; }
        public string DataStatus { get; set; } //for status
    }

    [Table("MSTEmployee")]
    public class ViewMSTEmployee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Username { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int PositionId { get; set; }
        public int DepartmentId { get; set; }
        public string ReportToEmpCode { get; set; }
        public string SignPicPath { get; set; }
        public string Lang { get; set; }
        public int AccountId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ADTitle { get; set; }
        public int? DivisionId { get; set; }
        public string EmpLevel { get; set; }
        public string EMPL_RCD { get; set; }
        public int? EmployeeLevel { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Userid_Line { get; set; }

    }

    public class ViewCloneMSTEmployee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Username { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int PositionId { get; set; }
        public int DepartmentId { get; set; }
        public string ReportToEmpCode { get; set; }
        public string SignPicPath { get; set; }
        public string Lang { get; set; }
        public int AccountId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ADTitle { get; set; }
        public int? DivisionId { get; set; }
        public string EmpLevel { get; set; }
        public string EMPL_RCD { get; set; }
        public int? EmployeeLevel { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Userid_Line { get; set; }
        public string DataStatus { get; set; } //for status

    }
    // Class สำหรับผลลัพธ์ที่ต้องการ
    public class EmployeeSuperiorDetail
    {
        [Key]
        public string EMP_CODE { get; set; }
        public string EmployeeCode { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public string DIVI_CODE { get; set; }
        public string DEPT_CODE { get; set; }
        public string SEC_CODE { get; set; }
        public string GRP_CODE { get; set; }
        public string POSITION { get; set; }
        public string CURRENT_POS { get; set; }
        public string CURRENT_HCM { get; set; }


        // Superior Info
        public string SUPERIOR_EMP_CODE { get; set; }
        public string SUPERIOR_DIVI_CODE { get; set; }
        public string SUPERIOR_DEPT_CODE { get; set; }
        public string SUPERIOR_SEC_CODE { get; set; }
        public string SUPERIOR_GRP_CODE { get; set; }
        public string SUPERIOR_POS_NAME { get; set; }
        public string SUPERIOR_HCM { get; set; }

    }

    [Table("MSTDepartment")]
    public class ViewMSTDepartment
    {
        [Key]
        public int DepartmentId { get; set; }
        public int? ParentId { get; set; }
        public int? DivisionId { get; set; }
        public string DepartmentCode { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public int? AccountId { get; set; }
        public int? LeaderId { get; set; }
        public string CompanyCode { get; set; }

    }
    [Table("MSTDivision")]
    public class ViewMSTDivision
    {
        [Key]
        public int DivisionId { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }
        public int? AccountId { get; set; }
        public string DivisionCode { get; set; }

    }

    [Table("MSTPosition")]
    public class ViewMSTPosition
    {
        [Key]
        public int PositionId { get; set; }
        public string NameTh { get; set; }
        public string NameEn { get; set; }
        public int PositionLevelId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? AccountId { get; set; }
        public string CompanyCode { get; set; }

    }


    [Table("MSTMasterData")]
    public class ViewMSTMasterData
    {

        [Key]
        public int MasterId { get; set; }
        public string MasterType { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? Seq { get; set; }


    }


}
