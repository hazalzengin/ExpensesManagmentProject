using Expenses.DataAccess;
using Expenses.DataAccess.Repositories;
using Expenses.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expenses.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpensesRepository _expense;

        public ExpensesController(IExpensesRepository expense)
        {
            _expense = expense;
        }

        public IActionResult Index(string searching, int pageNumber = 1, int pageSize = 10)
        {
            IEnumerable<ExpenseModel> filteredExpenses;

            if (string.IsNullOrEmpty(searching))
            {
                filteredExpenses = _expense.GetAllExpenses()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                filteredExpenses = _expense.Search(searching)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            ViewBag.TotalPages = (int)Math.Ceiling(_expense.GetAllExpenses().Count() / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.HasPreviousPage = (pageNumber > 1);
            ViewBag.HasNextPage = (pageNumber < ViewBag.TotalPages);

            return View(filteredExpenses.ToList());
        }


        [HttpGet]
        public IActionResult ExpenseData(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            ExpenseModel model = _expense.GetExpenseByID(id);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_ExpenseDataPartial", model);
        }

        [HttpPost]
        public IActionResult ExpenseData([FromBody] ExpenseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.Category))
            {
                ModelState.AddModelError("Category", "Category is required.");
                return BadRequest(ModelState);
            }

            try
            {
                if (model.Id > 0)
                {
                    _expense.Update(model);
                }
                else
                {
                    _expense.Add(model);
                }

                return PartialView("_ExpenseDataPartial", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult UpdateExpense([FromBody] ExpenseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _expense.Update(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            try
            {
                _expense.Delete(id);
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to delete due to database error.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
