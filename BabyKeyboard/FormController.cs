using System;
using System.Collections.Generic;

namespace BabyKeyboard
{
    public static class FormController
    {
        private static readonly List<FullScreenForm> forms = new List<FullScreenForm>();

        public static void RegisterForm(FullScreenForm form)
        {
            forms.Add(form);
        }

        public static void UnregisterForm(FullScreenForm form)
        {
            forms.Remove(form);
        }

        public static void BroadcastColorChange()
        {
            foreach (var form in forms)
            {
                form.ChangeColor();
            }
        }

        public static void BroadcastDrawShape()
        {
            foreach (var form in forms)
            {
                form.DrawShape();
            }
        }

        public static void BroadcastExit()
        {
            foreach (var form in forms.ToArray())
            {
                form.Close();
            }
        }
    }
}
