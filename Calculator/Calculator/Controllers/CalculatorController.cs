using Calculator.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
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
        public IActionResult Calculate(MainModel calculator)
        {
            calculator.Numbers = new List<int>();
            calculator.Signs = new List<string>();
            calculator = SerializationShizzle(calculator);
            if (calculator.Input == "0" && calculator.Signs.Contains("/"))
            {
                return Redirect("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            calculator = calculator.Numbers.Count > 1 ? CalculateValues(calculator) : calculator;

            calculator.Input = "";

            calculator = GenerateCalculationString(calculator);

            return View("Index", calculator);
        }
        private MainModel CalculateValues(MainModel mainModel)
        {

            var firstValue = mainModel.Numbers[0];
            if (mainModel.Result != null && mainModel.Numbers.Count > 2)
            {
                firstValue = Convert.ToInt32(mainModel.Result);
            }
            var secondValue = mainModel.Numbers[mainModel.Numbers.Count - 1];
            var sign = mainModel.Signs[mainModel.Signs.Count - 2];
            string serializedCalculator;
            switch (sign)
            {
                case "+":
                    mainModel.Result = firstValue + secondValue;
                    serializedCalculator = JsonConvert.SerializeObject(mainModel);
                    HttpContext.Session.SetString("calculator", serializedCalculator);
                    return mainModel;
                case "-":
                    mainModel.Result = firstValue - secondValue;
                    serializedCalculator = JsonConvert.SerializeObject(mainModel);
                    HttpContext.Session.SetString("calculator", serializedCalculator);
                    return mainModel;
                case "/":
                    mainModel.Result = firstValue / secondValue;
                    serializedCalculator = JsonConvert.SerializeObject(mainModel);
                    HttpContext.Session.SetString("calculator", serializedCalculator);
                    return mainModel;
                case "*":
                    mainModel.Result = firstValue * secondValue;
                    serializedCalculator = JsonConvert.SerializeObject(mainModel);
                    HttpContext.Session.SetString("calculator", serializedCalculator);
                    return mainModel;
                default:
                    serializedCalculator = JsonConvert.SerializeObject(mainModel);
                    HttpContext.Session.SetString("calculator", serializedCalculator);
                    return mainModel;
            }
        }
        private MainModel GenerateCalculationString(MainModel mainModel)
        {
            var index = mainModel.Signs.Count - 1;
            if (mainModel.Signs[mainModel.Signs.Count - 1] == "=")
            {
                if (mainModel.Numbers.Count == 2)
                {
                    mainModel.Calculation += mainModel.Numbers[index];
                }
                mainModel.Calculation += " " + mainModel.Signs[index] + " " + mainModel.Result;
            }
            else
            {
                if (mainModel.Numbers.Count > 1)
                {
                    mainModel.Calculation += " " + mainModel.Signs[index] + " " + mainModel.Numbers[index] + " ";
                }
                else
                {
                    mainModel.Calculation += mainModel.Numbers[index] + " " + mainModel.Signs[index] + " ";
                }
            }
            string serializedCalculator;
            serializedCalculator = JsonConvert.SerializeObject(mainModel);
            HttpContext.Session.SetString("calculator", serializedCalculator);
            return mainModel;
        }
        private MainModel SerializationShizzle(MainModel calculator)
        {
            var calculatorFromSession = HttpContext.Session.GetString("calculator");
            string serializedCalculator;
            if (calculatorFromSession != null)
            {
                var existingCalculator = JsonConvert.DeserializeObject<MainModel>(calculatorFromSession);
                existingCalculator.Numbers.Add(int.Parse(calculator.Input));
                existingCalculator.Signs.Add(calculator.Sign);
                existingCalculator.Sign = calculator.Sign;
                existingCalculator.Input = calculator.Input;
                serializedCalculator = JsonConvert.SerializeObject(existingCalculator);
                HttpContext.Session.SetString("calculator", serializedCalculator);
                return existingCalculator;
            }
            calculator.Numbers.Add(int.Parse(calculator.Input));
            calculator.Signs.Add(calculator.Sign);
            serializedCalculator = JsonConvert.SerializeObject(calculator);
            HttpContext.Session.SetString("calculator", serializedCalculator);
            return calculator;

        }
    }
}