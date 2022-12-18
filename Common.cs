using ClassTeacher_Assist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassTeacher_Assist
{
    internal static class Common
    {
        public static Teacher? CurrentUser;

        public static string GetTeachersClassCode(int teacherId)
        {
            PostgresContext db = new PostgresContext();
            try
            {
                return db.Classes.AsNoTracking().Where(c => c.TeacherId == teacherId).First().ClassCode;
            }
            catch (Exception ex)
            {
                return "";
            }
            
        }
    }
}
