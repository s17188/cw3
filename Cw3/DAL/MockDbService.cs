using System;
using System.Collections.Generic;
using Cw3.Models;
using System.Data.SqlClient;

namespace Cw3.DAL
{
    public class MockDbService : IDbService
    {
        /*private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent=1, FirstName="Jan", LastName="Kowalski"},
                new Student{IdStudent=2, FirstName="Anna", LastName="Malewski"},
                new Student{IdStudent=3, FirstName="Andrzej", LastName="Andrzejewicz"}
            };
        }
        */
        private const string DataSQLCon = "Data Source=db-mssql;Initial Catalog=s17188;Integrated Security=True";
        public IEnumerable<Student> GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student";

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();
                while (sqlRead.Read())
                {
                    var st = new Student();
                    st.FirstName = sqlRead["FirstName"].ToString();
                    st.LastName = sqlRead["LastName"].ToString();
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                    list.Add(st);
                }

            }
            return list;
        }

        public IEnumerable<Enrollment> GetStudentsEnrollment(string id)
        {

            var list = new List<Enrollment>();
            using (SqlConnection con = new SqlConnection(DataSQLCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT e.IdEnrollment,e.Semester,e.IdStudy,e.StartDate FROM Student AS s INNER JOIN Enrollment as e ON s.IdEnrollment = e.IdEnrollment WHERE s.IndexNumber = @id";
                com.Parameters.AddWithValue("id", id);

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();

                while (sqlRead.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = Int32.Parse(sqlRead["IdEnrollment"].ToString());
                    en.Semester = Int32.Parse(sqlRead["Semester"].ToString());
                    en.IdStudy = Int32.Parse(sqlRead["IdStudy"].ToString());
                    en.StartDate = sqlRead["StartDate"].ToString();
                    list.Add(en);
                }

            }

            return list;
        }

        public IEnumerable<EnrollStudent> AddNewStudentAndEnroll(EnrollStudent enrollStudent)
        {
            var std = new EnrollStudent();
            std.IndexNumber = enrollStudent.IndexNumber;
            std.FirstName = enrollStudent.FirstName;
            std.LastName = enrollStudent.LastName;
            std.BirthDate = enrollStudent.BirthDate.Replace('.', '/');
            std.Studies = enrollStudent.Studies;
            std.status = "Ok";


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
                        std.status = "Studia nie istnieja";
                        
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
                        std.status = "Semestr nie istnieje";
                        Random random = new Random();
                        localDate = DateTime.Now;
                        int num = random.Next(20);
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
                        std.enrollment = enrollment;
                        com.CommandText = "select IndexNumber from Student where IndexNumber=@IndexNumber";
                        com.Parameters.AddWithValue("IndexNumber", std.IndexNumber);
                        var student = com.ExecuteReader();
                        if (!student.Read())
                        {
                            student.Close();
                            tran.Rollback();
                            std.status = "Student istnieje";
                        }
                        else
                        {
                            std.status = "Student dodany";
                            student.Close();
                            com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, idEnrollment) VALUES(@Index, @Fname, @Lname, @Bdate, @idEnrollment)";
                            com.Parameters.AddWithValue("Index", std.IndexNumber);
                            com.Parameters.AddWithValue("Fname", std.FirstName);
                            com.Parameters.AddWithValue("Lname", std.LastName);
                            com.Parameters.AddWithValue("idEnrollment", idenrollment);
                            com.Parameters.AddWithValue("Bdate", sqlFormattedDate);
                            com.ExecuteNonQuery();

                            tran.Commit();

                        }
                    }
                    catch (SqlException exc)
                    {
                        tran.Rollback();
                    }

                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    std.status = exc.Message.ToString();
                }
            }
            yield return std;
        }
    }
}
