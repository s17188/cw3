using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentDbService
    {
        public SqlServerDbService(){
        }
        private const string DataSQLCon = "Data Source=db-mssql;Initial Catalog=s17188;Integrated Security=True";
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollStudent)
        {
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

            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    com.CommandText = "select idStudy from Studies where Name=@Name";
                    com.Parameters.AddWithValue("Name", std.Studies);
                    com.Transaction = tran;
                    var stud = com.ExecuteReader();
                    if (!stud.Read())
                    {
                        tran.Rollback();
                        response.status = "Studia nie istnieja";

                    }
                    int idstudies = (int)stud["idStudy"];
                    stud.Close();
                    System.Diagnostics.Debug.WriteLine("STUD AFTER");
                    com.CommandText = "select idEnrollment,StartDate from Enrollment where idStudy=@idStudy order by StartDate";
                    com.Parameters.AddWithValue("idStudy", idstudies);
                    int idenrollment;
                    DateTime localDate;
                    var enroll = com.ExecuteReader();
                    if (!enroll.Read())
                    {
                        response.status = "Semestr nie istnieje";
                        Random random = new Random();
                        localDate = DateTime.Now;
                        int num = random.Next(4,20);
                        idenrollment = num;
                        com.CommandText = "INSERT INTO Enrollment VALUES(@id,@Semestr,@IdStudies,@StartDate)";
                        com.Parameters.AddWithValue("id", num);
                        com.Parameters.AddWithValue("Semestr", 1);
                        com.Parameters.AddWithValue("IdStudies", idstudies);
                        com.Parameters.AddWithValue("StartDate", localDate);
                        enroll.Close();
                        System.Diagnostics.Debug.WriteLine("SEMM");
                        com.ExecuteNonQuery();

                    }
                    else
                    {
                        idenrollment = (int)enroll["IdEnrollment"];
                        localDate = (DateTime)enroll["StartDate"];
                        enroll.Close();
                    }

                    try
                    {

                        Enrollment enrollment = new Enrollment();
                        enrollment.IdEnrollment = idenrollment;
                        enrollment.IdStudy = idstudies;
                        enrollment.Semester = 1;
                        enrollment.StartDate = localDate.ToString();
                        response.enrollment = enrollment;
                        com.CommandText = "select IndexNumber from Student where IndexNumber=@IndexNumber";
                        com.Parameters.AddWithValue("IndexNumber", std.IndexNumber);

                        response.status = "Student dodany";
                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, idEnrollment) VALUES(@Index, @Fname, @Lname, @Bdate, @idEnrollment)";
                        com.Parameters.AddWithValue("Index", std.IndexNumber);
                        com.Parameters.AddWithValue("Fname", std.FirstName);
                        com.Parameters.AddWithValue("Lname", std.LastName);
                        com.Parameters.AddWithValue("idEnrollment", idenrollment);
                        com.Parameters.AddWithValue("Bdate", sqlFormattedDate);
                        com.ExecuteNonQuery();

                        tran.Commit();

                    }
                    catch (SqlException exc)
                    {
                        tran.Rollback();
                        response.status = "Student istnieje lub wystapil blad w bazie: " + exc.Message.ToString();
                    }

                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    response.status = exc.Message.ToString();
                }
                return response;
            }
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            using (var conn = new SqlConnection(DataSQLCon))
            using (var command = new SqlCommand("PromoteStudents", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add(new SqlParameter("@Studies",studies));
                command.Parameters.Add(new SqlParameter("@Semester", semester));

                var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                conn.Open();
                command.ExecuteNonQuery();
                var result = returnParameter.Value;
                System.Diagnostics.Debug.WriteLine("home " + result);

                conn.Close();

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                conn.Open();

                com.CommandText = "select idEnrollment,idStudy,Semester,StartDate from Enrollment where idEnrollment=@idEnrollment";
                com.Parameters.AddWithValue("idEnrollment", result);


                Enrollment enrollment = new Enrollment();
                var enroll = com.ExecuteReader();
                if (enroll.Read())
                {
                    enrollment.IdEnrollment = (int)enroll["IdEnrollment"];
                    enrollment.IdStudy = (int)enroll["IdStudy"];
                    enrollment.Semester = (int)enroll["Semester"];
                    enrollment.StartDate = (string)enroll["StartDate"].ToString();
                    enroll.Close();
                }
                else
                {
                    enroll.Close();
                }
                conn.Close();
                return enrollment;
            }
        }

        public Student GetStudent(string index)
        {
            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber from Student WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", index);

                con.Open();
                var st = new Student();
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
    }
}
