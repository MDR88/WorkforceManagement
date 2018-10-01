﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using BangazonWebApp.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Workforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            select
                s.Id,
                s.Name,
                s.Budget,                
            from Departments s
            WHERE s.Id = {id}";

            using (IDbConnection conn = Connection)
            {

                Departments departments = (await conn.QueryAsync<Departments>(sql)).ToList().Single();

                if (departments == null)
                {
                    return NotFound();
                }

                return View(departments);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([Bind ("DepartmentId, Name, Budget")]Departments departments)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Departments
                        ( Name, Budget)
                        VALUES
                        ( null,
                             '{departments.Name}'
                            , '{departments.Budget}'
                            
                        )
                    ";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(departments);


        }


    }
}