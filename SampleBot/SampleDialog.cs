﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;

namespace SampleBot
{ 
    [LuisModel("c413b2ef-382c-45bd-8ff0-f76d60e2a821", "6d0966209c6e4f6b835ce34492f3e6d9")]
    [Serializable]
    internal class SampleDialog : LuisDialog<Employee>
    {
        private static IForm<Employee> BuildForm()
        {
            var builder = new FormBuilder<Employee>();
            return builder
                .AddRemainingFields()
                .Build();
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }


        [LuisIntent("builtin.intent.communication.send_text")]
        public async Task TextEmployee(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            //EntityRecommendation entityName;
            //result.TryFindEntity("builtin.communication.contact_name", out entityName);
            //if (entityName != null)
            //    entities.Add(new EntityRecommendation(type: "Name") { Entity = entityName.Entity });
            var unlockForm = new FormDialog<Employee>(new Employee(), BuildForm, FormOptions.PromptInStart, entities);
            context.Call(unlockForm, TextEmployeeComplete);
        }

        private async Task TextEmployeeComplete(IDialogContext context, IAwaitable<Employee> result)
        {
            try
            {
                var emp = await result;

                await context.PostAsync($"Success for {emp.Name}");

                context.Wait(MessageReceived);
            }
            catch (FormCanceledException<Employee> e)
            {
                string reply;
                if (e.InnerException == null)
                {
                    reply = $"You quit --maybe you can finish next time!";
                }
                else
                {
                    reply = e.InnerException.Message;
                }
                await context.PostAsync(reply);
            }
        }
    }
}