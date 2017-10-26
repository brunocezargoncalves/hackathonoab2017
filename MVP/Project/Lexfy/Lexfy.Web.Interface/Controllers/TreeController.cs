using Lexfy.Application.Interfaces;
using Lexfy.Domain;
using Lexfy.Web.Interface.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Lexfy.Web.Interface.Controllers
{
    public class TreeController : Controller
    {
        private readonly ITreeApplication _treeApplication;

        public TreeController(ITreeApplication treeApplication)
        {
            _treeApplication = treeApplication;
        }

        // GET: Tree
        public ActionResult Index()
        {
            return View(new TreeViewModel());
        }

        // GET: Tree/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tree/Create
        public ActionResult Create()
        {
            return View(FillFields(string.Empty));
        }

        // POST: Tree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TreeViewModel treeViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(AddError(treeViewModel, string.Empty));

                Save("Create", treeViewModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(AddError(treeViewModel, ex.Message));
            }
        }

        // GET: Tree/Edit/5
        public ActionResult Edit(string treeId)
        {
            return View(FillFields(treeId));
        }

        // POST: Tree/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tree/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tree/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region Helpers

        public JsonResult Search()
        {
            var trees = _treeApplication.Find(new Tree
            {
                TreeId = Guid.Empty                
            }).Select(tree => new string[] {
                tree.TreeId.ToString(),
                tree.Title}).ToList();

            return Json(new
            {
                aaData = trees
            }, JsonRequestBehavior.AllowGet);
        }

        private TreeViewModel FillFields(string treeId)
        {
            var trees = _treeApplication.Find(null).Select(item => new SelectListItem() { Text = item.Title, Value = item.TreeId.ToString() }).ToList();
            var tree = !string.IsNullOrEmpty(treeId) ? _treeApplication.Get(Guid.Parse(treeId)) : null;

            if (tree != null)
            {
                return new TreeViewModel()
                {
                    TreeId = tree.TreeId,
                    Title = tree.Title,
                    Description = tree.Description,
                    TreesChild = trees,
                    SelectedTreeChild = tree.TreeId.ToString()                    
                };
            }

            return new TreeViewModel()
            {
                TreesChild = trees,
                //SelectedTreeChild = ""
            };
        }

        private void Save(string caller, TreeViewModel treeViewModel)
        {
            try
            {
                var tree = new Tree()
                {
                    TreeId = treeViewModel.TreeId,
                    Title = treeViewModel.Title,
                    Description = treeViewModel.Description,
                };

                switch (caller)
                {
                    case "Create":
                    case "Edit":
                        _treeApplication.Save(tree);
                        break;
                    case "Delete":
                        _treeApplication.SoftDelete(tree);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private TreeViewModel AddError(TreeViewModel treeViewModel, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
                ModelState.AddModelError("TreeErrorMessage", errorMessage);

            return treeViewModel;
        }

        #endregion Helpers
    }
}
