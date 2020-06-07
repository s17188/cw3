using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SimpleCrypto;
using Cw3.ModelsGenerated;
using Microsoft.EntityFrameworkCore;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentDbService
    {
        public IConfiguration Configuration { get; }

        public SqlServerDbService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private const string DataSQLCon = "Data Source=db-mssql;Initial Catalog=s17188;Integrated Security=True";
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollStudent)
        {
            var db = new s17188Context();
            EnrollStudentResponse response = new EnrollStudentResponse();
            response.LastName = enrollStudent.LastName;
            response.IndexNumber = enrollStudent.IndexNumber;
            var std = new EnrollStudentRequest();
            std.IndexNumber = enrollStudent.IndexNumber;
            std.FirstName = enrollStudent.FirstName;
            std.LastName = enrollStudent.LastName;
            std.BirthDate = enrollStudent.BirthDate.Replace('.', '/');
            std.Studies = enrollStudent.Studies;
            response.status = "Ok";


            DateTime myDateTime = Convert.ToDateTime(std.BirthDate);
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            var study = db.Studies
                            .Where(s => s.Name == std.Studies);

            if(study.Count() == 0)
            {
                response.status = "Studia nie istnieja";
            }

            int idstudies = study.Single().IdStudy;
            var enrollment = db.Enrollment
                                .Where(e => e.IdStudy == idstudies)
                                .OrderBy(e => e.StartDate);
            int idenrollment;
            DateTime localDate;
            if (enrollment.Count() > 0) {
                response.status = "Semestr nie istnieje";
                Random random = new Random();
                localDate = DateTime.Now;
                int num = random.Next(4, 20);
                idenrollment = num;
                var newEnrollment = new ModelsGenerated.Enrollment
                {
                    IdEnrollment = idenrollment,
                    Semester = 1,
                    IdStudy = idstudies,
                    StartDate = localDate
                };
                db.Enrollment.Add(newEnrollment);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception exc)
                {
                    response.status = "Semestr istnieje " + exc.Message.ToString();
                }
            }
            else
            {
                idenrollment = enrollment.Single().IdEnrollment;
                localDate = (DateTime)enrollment.Single().StartDate;
            }

            ModelsGenerated.Enrollment enroll = new ModelsGenerated.Enrollment();
            enroll.IdEnrollment = enrollment.FirstOrDefault().IdEnrollment;
            enroll.IdStudy = enrollment.First().IdStudy;
            enroll.Semester = enrollment.First().Semester;
            enroll.StartDate = enrollment.First().StartDate;
            response.enrollment = enroll;
            response.status = "Student dodany";

            var newStudent = new ModelsGenerated.Student
            {
                IndexNumber = std.IndexNumber,
                FirstName = std.FirstName,
                LastName = std.LastName,
                IdEnrollment = idenrollment,
                BirthDate = localDate
            };

            db.Student.Add(newStudent);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exc)
            {
                response.status = "Student istnieje lub wystapil blad w bazie: " + exc.Message.ToString();
            }
            return response;

            //using (SqlConnection con = new SqlConnection(DataSQLCon))
            //using (SqlCommand com = new SqlCommand())
            //{

            //    com.Connection = con;
            //    con.Open();
            //    var tran = con.BeginTransaction();
            //    try
            //    {
            //        com.CommandText = "select idStudy from Studies where Name=@Name";
            //        com.Parameters.AddWithValue("Name", std.Studies);
            //        com.Transaction = tran;
            //        var stud = com.ExecuteReader();
            //        if (!stud.Read())
            //        {
            //            tran.Rollback();
            //            response.status = "Studia nie istnieja";

            //        }
            //        int idstudies = (int)stud["idStudy"];
            //        stud.Close();
            //        System.Diagnostics.Debug.WriteLine("STUD AFTER");
            //        com.CommandText = "select idEnrollment,StartDate from Enrollment where idStudy=@idStudy order by StartDate";
            //        com.Parameters.AddWithValue("idStudy", idstudies);
            //        int idenrollment;
            //        DateTime localDate;
            //        var enroll = com.ExecuteReader();
            //        if (!enroll.Read())
            //        {
            //            response.status = "Semestr nie istnieje";
            //            Random random = new Random();
            //            localDate = DateTime.Now;
            //            int num = random.Next(4,20);
            //            idenrollment = num;
            //            com.CommandText = "INSERT INTO Enrollment VALUES(@id,@Semestr,@IdStudies,@StartDate)";
            //            com.Parameters.AddWithValue("id", num);
            //            com.Parameters.AddWithValue("Semestr", 1);
            //            com.Parameters.AddWithValue("IdStudies", idstudies);
            //            com.Parameters.AddWithValue("StartDate", localDate);
            //            enroll.Close();
            //            System.Diagnostics.Debug.WriteLine("SEMM");
            //            com.ExecuteNonQuery();

            //        }
            //        else
            //        {
            //            idenrollment = (int)enroll["IdEnrollment"];
            //            localDate = (DateTime)enroll["StartDate"];
            //            enroll.Close();
            //        }

            //        try
            //        {

            //            Models.Enrollment enrollment = new Models.Enrollment();
            //            enrollment.IdEnrollment = idenrollment;
            //            enrollment.IdStudy = idstudies;
            //            enrollment.Semester = 1;
            //            enrollment.StartDate = localDate.ToString();
            //            response.enrollment = enrollment;
            //            com.CommandText = "select IndexNumber from Student where IndexNumber=@IndexNumber";
            //            com.Parameters.AddWithValue("IndexNumber", std.IndexNumber);

            //            response.status = "Student dodany";
            //            com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, idEnrollment) VALUES(@Index, @Fname, @Lname, @Bdate, @idEnrollment)";
            //            com.Parameters.AddWithValue("Index", std.IndexNumber);
            //            com.Parameters.AddWithValue("Fname", std.FirstName);
            //            com.Parameters.AddWithValue("Lname", std.LastName);
            //            com.Parameters.AddWithValue("idEnrollment", idenrollment);
            //            com.Parameters.AddWithValue("Bdate", sqlFormattedDate);
            //            com.ExecuteNonQuery();

            //            tran.Commit();

            //        }
            //        catch (SqlException exc)
            //        {
            //            tran.Rollback();
            //            response.status = "Student istnieje lub wystapil blad w bazie: " + exc.Message.ToString();
            //        }

            //    }
            //    catch (SqlException exc)
            //    {
            //        tran.Rollback();
            //        response.status = exc.Message.ToString();
            //    }
            //    return response;
            //}
        }

        public ModelsGenerated.Enrollment PromoteStudents(int semester, string studies)
        {

            var db = new s17188Context();
            //var parameters = new { studies, semester };
            //var affectedDatas = db.Database.ExecuteSqlCommand("PromoteStudents @Studies, @Semester", parameters);
            //object[] xparams = {
            //    new SqlParameter("@Studies", studies),
            //    new SqlParameter("@Semester", semester),
            //    new SqlParameter("@ReturnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}};

            //SqlParameter param1 = new SqlParameter("@Studies", studies);
            //SqlParameter param2 = new SqlParameter("@Semester", semester);
            //SqlParameter param3 = new SqlParameter("@ReturnVal", SqlDbType.Int);

            //var ReturnValue = ((SqlParameter)xparams[2]).Value;

            using (var conn = new SqlConnection(DataSQLCon))
            using (var command = new SqlCommand("PromoteStudents", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add(new SqlParameter("@Studies", studies));
                command.Parameters.Add(new SqlParameter("@Semester", semester));

                var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                conn.Open();
                command.ExecuteNonQuery();
                var result = returnParameter.Value;
                System.Diagnostics.Debug.WriteLine("home " + result);

                conn.Close();

                var enroll = db.Enrollment
                .Where(e => e.IdEnrollment == (int)result)
                .Single();

                return enroll;

            }
            //SqlCommand com = new SqlCommand();
            //com.Connection = conn;
            //conn.Open();

            //com.CommandText = "select idEnrollment,idStudy,Semester,StartDate from Enrollment where idEnrollment=@idEnrollment";
            //com.Parameters.AddWithValue("idEnrollment", result);


            //Models.Enrollment enrollment = new Models.Enrollment();
            //var enroll = com.ExecuteReader();
            //if (enroll.Read())
            //{
            //    enrollment.IdEnrollment = (int)enroll["IdEnrollment"];
            //    enrollment.IdStudy = (int)enroll["IdStudy"];
            //    enrollment.Semester = (int)enroll["Semester"];
            //    enrollment.StartDate = (string)enroll["StartDate"].ToString();
            //    enroll.Close();
            //}
            //else
            //{
            //    enroll.Close();
            //}
            //conn.Close();



        }

        public Models.Student GetStudent(string index)
        {
            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber from Student WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", index);

                con.Open();
                var st = new Models.Student();
                SqlDataReader sqlRead = com.ExecuteReader();
                if (sqlRead.Read())
                {
                    
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                    return st;
                }
                else
                {
                    return null;
                }
            }
        }

        public LoginResponse LoginStudent(string login,string haslo)
        {
            ICryptoService cryptoService = new PBKDF2();
            
            var st = new Models.Student();
            var resp = new LoginResponse();

            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber,Password,salt from Student WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", login);

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();
                if (sqlRead.Read())
                {
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                    string BaseSalt = sqlRead["salt"].ToString();
                    string password = sqlRead["Password"].ToString();
                    string hasloLocal = cryptoService.Compute(haslo, BaseSalt);
                    bool isPasswordValid = cryptoService.Compare(password, hasloLocal);
                    if (!isPasswordValid)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                con.Close();
                
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                resp.accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                resp.refreshToken = Guid.NewGuid();

                con.Open();
                com.CommandText = "UPDATE Student SET refreshToken=@Refresh WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Refresh", resp.refreshToken);

                com.ExecuteNonQuery();
                con.Close();

            }

            return resp;
        }

        public LoginResponse RefreshToken(string refToken)
        {
            var st = new Models.Student();
            var resp = new LoginResponse();

            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber from Student WHERE refreshToken=@refToken";
                com.Parameters.AddWithValue("refToken", refToken);

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();
                if (sqlRead.Read())
                {
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                }
                else
                {
                    return null;
                }
                con.Close();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                resp.accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                resp.refreshToken = Guid.NewGuid();

                con.Open();
                com.CommandText = "UPDATE Student SET refreshToken=@Refresh WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", st.IndexNumber);
                com.Parameters.AddWithValue("Refresh", resp.refreshToken);

                com.ExecuteNonQuery();
                con.Close();

            }

            return resp;
        }


    }
}
