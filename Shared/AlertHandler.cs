using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    static class AlertHandler
    {
        static Stack<MessageInfo> pending = new Stack<MessageInfo>();
        static public async Task<int?> ShowMessage(string title, string text, string[] buttons)
        {
            int? ret = null;
            if (MessageBox.IsVisible)
            {
                MessageInfo info = new MessageInfo(title, text, buttons);
                TaskCompletionSource<int?> result = new TaskCompletionSource<int?>();
                info.OnCompleted += (res) =>
                  {
                      result.SetResult(res);
                  };
                pending.Push(info);
                ret = await result.Task;
            }
            else
                ret = await MessageBox.Show(title, text, buttons);
            CheckNext();
            return ret;
        }
        static internal async Task CheckNext()
        {
            if (pending.Count > 0 && !MessageBox.IsVisible)
            {
                MessageInfo info = pending.Pop();
                info.OnCompleted(await MessageBox.Show(info.Title, info.Text, info.Buttons));
            }
        }
    }
    class MessageInfo
    {
        public MessageInfo(string title, string text, string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons;
        }

        public string Title { get; set; }
        public string Text { get; set; }
        public string[] Buttons { get; set; }

        public Action<int?> OnCompleted;
    }
}
