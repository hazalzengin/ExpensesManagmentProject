using Expenses.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expenses.DataAccess.Repositories
{
    public class ExpensesRepository : IExpensesRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpensesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(ExpenseModel expense)
        {
            try
            {
                _context.ExpensesTable.Add(expense);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            var expenseToDelete = _context.ExpensesTable.Find(id);
            if (expenseToDelete != null)
            {
                _context.ExpensesTable.Remove(expenseToDelete);
                _context.SaveChanges();
            }
        }

        public IEnumerable<ExpenseModel> GetAllExpenses()
        {
            try
            {
                var expenses = _context.ExpensesTable.ToList();
                return expenses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ExpenseModel GetExpenseByID(int id)
        {
            try
            {
                var expense = _context.ExpensesTable.Find(id);
                return expense;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ExpenseModel> Search(string searchString)
        {
            try
            {
                var searchExpenses = GetAllExpenses().Where(x => x.Category.Contains(searchString)).ToList();
                return searchExpenses;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public int Update(ExpenseModel expense)
        {
            try
            {
                _context.Entry(expense).State = EntityState.Modified;
                _context.SaveChanges();
                return 1;
            }
            catch (DbUpdateException ex)
            {
               
                Console.WriteLine($"Error updating expense: {ex.Message}");
                return -1; 
            }
        }

    }
}

