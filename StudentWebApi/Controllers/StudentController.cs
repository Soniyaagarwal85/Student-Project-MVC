using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentWebApi.Models;

namespace StudentWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly MyDbContext db;

        public StudentController(MyDbContext db)
        {
            this.db = db;
        }

            // GET: api/students
            [HttpGet]
            public async Task<ActionResult> GetStudents(string selectedCourse, string sortOrder, int page = 1, int pageSize = 5)
            {
                // Get the students from the database
                var students = db.Students.AsQueryable();

                // Filter by selected course
                if (!string.IsNullOrEmpty(selectedCourse))
                {
                    students = students.Where(s => s.Course == selectedCourse);
                }

                // Apply sorting based on the selected order
                var sortedStudents = SortStudents(students, sortOrder);

                // Calculate pagination
                int totalRecords = await sortedStudents.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Get the paged data
                var pagedData = await sortedStudents
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Fetch distinct courses for the dropdown
                var courses = await db.Students
                    .Select(s => s.Course)
                    .Distinct()
                    .ToListAsync();

                // Return a response with pagination info, courses, and the paged data
                var response = new
                {
                    TotalPages = totalPages,
                    CurrentPage = page,
                    Courses = courses,
                    Students = pagedData
                };

                return Ok(response);
            }

            // Optional: Add a method to get a student by ID
            [HttpGet("{id}")]
            public async Task<ActionResult<Student>> GetStudentById(int id)
            {
                var student = await db.Students.FindAsync(id);
                if (student == null)
                {
                    return NotFound(); // Return 404 if the student is not found
                }
                return Ok(student); // Return the student if found
            }

            // Helper method for sorting
            private IQueryable<Student> SortStudents(IQueryable<Student> students, string sortOrder)
            {
                switch (sortOrder)
                {
                case "name_desc":
                    return students.OrderByDescending(s => s.Name);
                    break;
                case "name_asc":
                    return students.OrderBy(s => s.Name);
                    break;
                case "age_desc":
                    return students.OrderByDescending(s => s.Age);
                    break;
                case "age_asc":
                    return students.OrderBy(s => s.Age);
                    break;
                case "gender_desc":
                    return students.OrderByDescending(s => s.Gender);
                    break;
                case "gender_asc":
                    return students.OrderBy(s => s.Gender);
                    break;
                case "email_desc":
                    return students.OrderByDescending(s => s.Email);
                    break;
                case "email_asc":
                    return students.OrderBy(s => s.Email);
                    break;
                case "phone_desc":
                    return students.OrderByDescending(s => s.Phone);
                    break;
                case "phone_asc":
                    return students.OrderBy(s => s.Phone);
                    break;
                case "course_desc":
                    return students.OrderByDescending(s => s.Course);
                    break;
                case "course_asc":
                    return students.OrderBy(s => s.Course);
                    break;
                default:
                    return students.OrderBy(s => s.Name);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student stud)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            await db.Students.AddAsync(stud);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentById), new { id = stud.StudentId }, stud);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student stud)
        {
            Console.WriteLine("PUT method called");

            if (id != stud.StudentId)
                return BadRequest("Mismatched ID");

            var student = await db.Students.FindAsync(id);
            if (student == null)
                return NotFound("Student not found");

            student.Name = stud.Name;
            student.Age = stud.Age;
            student.Gender = stud.Gender;
            student.Email = stud.Email;
            student.Phone = stud.Phone;
            student.Course = stud.Course;

            await db.SaveChangesAsync();
            Console.WriteLine("Student updated successfully");

            return Ok(student);
        }


        //[HttpGet]
        //public async Task<ActionResult<List<Student>>> GetStudents()
        //{
        //    var data = await db.Students.ToListAsync();
        //    return Ok(data);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Student>> GetStudentsById(int id)
        //{
        //    var student = await db.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    return student;
        //}

        //[HttpPost]
        //public async Task<ActionResult<Student>> CreateStudent(Student stud)
        //{
        //    await db.Students.AddAsync(stud);
        //    await db.SaveChangesAsync();
        //    return Ok(stud);
        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult<Student>> UpdateStudent(int id,Student stud)
        //{
        //    if(id != stud.StudentId)
        //    {
        //        return BadRequest();
        //    }
        //    db.Entry(stud).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //    return Ok(stud);
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Student>> DeleteStudent(int id)
        //{
        //    var stud = await db.Students.FindAsync(id);
        //    if(stud == null)
        //    {
        //        return NotFound();
        //    }
        //    db.Students.Remove(stud);
        //    await db.SaveChangesAsync();
        //    return Ok();
        //}
    }
}
