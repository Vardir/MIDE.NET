﻿using MIDE.Application;
using MIDE.API.Commands;
using MIDE.API.Components;
using MIDE.API.Validations;
using MIDE.Application.Initializers;

namespace MIDE.WPFApp.Initializers
{
    public class ApplicationMenuInitializer : BaseMenuInitializer
    {
        public ApplicationMenuInitializer(AppKernel appKernel) : base(appKernel) { }

        protected override void PopulateMenu(IMenuConstructionContext context)
        {
            context.AddItem(new MenuButton("file", -99));
            context.AddItem("file", new MenuGroup("file-basic", -99));
            context.AddItem("file", new MenuGroup("file-exit", 99));

            context.AddItem("file/file-basic", new MenuButton("new", -99));
            context.AddItem("file/file-basic/new", new MenuButton("file", -99));
            context.AddItem("file/file-basic/new", new MenuButton("folder", -98));
            context.AddItem("file/file-basic/new", new MenuButton("project", -97));
            context.AddItem("file/file-basic", new MenuButton("open", -98));
            context.AddItem("file/file-basic/open", new MenuButton("file", -99));
            context.AddItem("file/file-basic/open", new MenuButton("folder", -98));
            context.AddItem("file/file-basic/open", new MenuButton("project", -97));

            context.AddItem("file/file-exit", new MenuButton("exit", 99)
            {
                PressCommand = new RelayCommand(appKernel.Exit)
            });

            context.AddItem(new MenuButton("edit", -98));
            context.AddItem(new MenuButton("view", -97));
            context.AddItem(new MenuButton("tools", 50));
            context.AddItem(new MenuButton("help", 99));
#if DEBUG
            context.AddItem("help", new MenuButton("about", -99)
            {
                PressCommand = new RelayCommand(() =>
                {
                    var res = DialogWindow.Show(new MessageDialogBox("About", "WPF template application"));
                })
            });
            context.AddItem("help", new MenuButton("my-question", 0)
            {
                PressCommand = new RelayCommand(() =>
                {
                    var res = DialogWindow.Show(new QuestionDialogBox("Question", "Do you have a laptop?"));
                    DialogWindow.Show(new MessageDialogBox("Answer", $"Your answer: {res.result}"));
                })
            });
            context.AddItem("help", new MenuButton("survey", 0)
            {
                PressCommand = new RelayCommand(() =>
                {
                    var box = new TextBoxDialogBox("Write your answer", "Your name:");
                    var validation = new DefaultStringPropertyValidation(false, false, "^[a-zA-Zа-яА-Я]*$");
                    validation.AttachTo(box.Input);
                    var res = DialogWindow.Show(box);
                    if (res.result == DialogResult.Accept)
                        DialogWindow.Show(new MessageDialogBox("Answer", $"Your answer: {res.value}"));
                })
            });
#endif
        }
    }
}