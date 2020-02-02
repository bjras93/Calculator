using Calculator.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calculator.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            var calculatorFromSession = HttpContext.Session.GetString("calculator");
            if (calculatorFromSession != null)
            {
                var existingCalculator = JsonConvert.DeserializeObject<MainModel>(calculatorFromSession);
                return View(existingCalculator);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Calculate(MainModel mainModel)
        {
            mainModel.Numbers = new List<int>();
            mainModel.Signs = new List<string>();


            if (mainModel.Input == 0 && mainModel.Signs.Contains("/"))
            {
                return Redirect("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            mainModel = GetCurrentCalculator(mainModel);
            if (mainModel.Input != null)
            {
                mainModel = CalculateValues(mainModel);
            }
            if (mainModel.Sign == "C")
            {
                HttpContext.Session.Clear();
                return View("Index", null);
            }

            SetCurrentCalculator(mainModel);

            mainModel = GenerateCalculationString(mainModel);
            mainModel.Input = null;
            ModelState.Clear();
            return View("Index", mainModel);
        }
        private MainModel CalculateValues(MainModel mainModel)
        {
            var sign = mainModel.Sign == "=" ? mainModel.Signs[mainModel.Signs.Count - 2] : mainModel.Sign;
            if (mainModel.Result == null)
            {
                mainModel.Result = mainModel.Input;
                SetCurrentCalculator(mainModel);
                return mainModel;
            }
            switch (sign)
            {
                case "+":
                    mainModel.Result += mainModel.Input;
                    break;
                case "-":
                    mainModel.Result -= mainModel.Input;
                    break;
                case "/":
                    mainModel.Result /= mainModel.Input;
                    break;
                case "*":
                    mainModel.Result *= mainModel.Input;
                    break;
                default:
                    break;
            }
            SetCurrentCalculator(mainModel);
            return mainModel;
        }
        private MainModel GenerateCalculationString(MainModel mainModel)
        {
            if (mainModel.Sign == "=")
            {
                mainModel.Calculation += $" {mainModel.Input} {mainModel.Sign} {mainModel.Result} ";
            }
            else if (mainModel.Signs.Count > 1 && mainModel.Signs[mainModel.Signs.Count - 2] == "=")
            {
                mainModel.Calculation += $" {mainModel.Sign} {mainModel.Input} ";
            }
            else
            {
                mainModel.Calculation += $" {mainModel.Input} {mainModel.Sign} ";
            }

            SetCurrentCalculator(mainModel);
            return mainModel;
        }
        private MainModel GetCurrentCalculator(MainModel mainModel)
        {
            var mainModelFromSession = HttpContext.Session.GetString("mainModel");

            if (mainModelFromSession != null)
            {
                var lastMainModel = JsonConvert.DeserializeObject<MainModel>(mainModelFromSession);

                lastMainModel.Input = mainModel.Input;
                lastMainModel.Sign = mainModel.Sign;
                mainModel = lastMainModel;
            }
            mainModel.Numbers.Add(mainModel.Input.GetValueOrDefault());
            mainModel.Signs.Add(mainModel.Sign);
            return mainModel;
        }
        private void SetCurrentCalculator(MainModel mainModel)
        {
            var serialized = JsonConvert.SerializeObject(mainModel);
            HttpContext.Session.SetString("mainModel", serialized);
        }

    }
}