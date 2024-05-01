using Expenses.DataAccess;
using Expenses.DataAccess.Repositories;
using Expenses.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index(string searching)
        {
            List<ExpenseModel> lists = new List<ExpenseModel>();

            if (string.IsNullOrEmpty(searching))
            {
                lists = _expense.GetAllExpenses().ToList();
            }
            else
            {
                lists = _expense.Search(searching).ToList();
            }
            return View(lists);
        }

        [HttpGet]
        public IActionResult ExpenseData(int id)
        {
            ExpenseModel model = new ExpenseModel();
            if (id > 0)
            {
                model = _expense.GetExpenseByID(id);
            }
            return PartialView("Index", model);
        }

        [HttpPost]

        public IActionResult ExpenseData([FromBody] ExpenseModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.Category))
                {
                    if (model.Id > 0)
                    {
                        _expense.Update(model);
                    }
                    else
                    {
                        _expense.Add(model);

                    }

                }
                else
                {
                    ModelState.AddModelError("Category", "Category is required.");
                }
            }
            return PartialView("Index", model);
        }

        [HttpPost]
        public IActionResult UpdateExpense([FromBody] ExpenseModel model)
        {
            if (ModelState.IsValid)
            {
                _expense.Update(model);
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            try
            {
             
                _expense.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

    }
}
