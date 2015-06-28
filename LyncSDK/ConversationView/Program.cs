using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Extensibility;

namespace ConversationView
{
	class Program
	{
		static void Main(string[] args)
		{
			Automation automation = LyncClient.GetAutomation();

			// IMを送る相手を設定する。
			// 複数人に送る場合にはその人数だけリストに追加
			List<string> inviteeList = new List<string>();
			inviteeList.Add("ninomiya@sample.com");

			// IM送信時のオプション設定を構築
			var modalitySettings = new Dictionary<AutomationModalitySettings, object>();
			modalitySettings.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);

			// 呼び出し
			IAsyncResult ar = automation.BeginStartConversation(
				AutomationModalities.InstantMessage
				, inviteeList
				, modalitySettings
				, null
				, null);

			//新しく開いたウィンドを取得
			ConversationWindow conversationWindow = automation.EndStartConversation(ar);

			//IM送信に関わるオブジェクトの取得
			//InstantMessageModality imModality = conversationWindow.Conversation.Modalities[ModalityTypes.InstantMessage] as InstantMessageModality;

			//メッセージ受信の処理を、IM参加者ごとにイベントを登録
            foreach (Participant participant in conversationWindow.Conversation.Participants)
			{
				// IM を取得
				InstantMessageModality im = participant.Modalities[ModalityTypes.InstantMessage] as InstantMessageModality;

				// IM 受信の際のイベント処理を登録
				im.InstantMessageReceived += ImModality_InstantMessageReceived;
			}

			Console.ReadKey();
		}

		private static void ImModality_InstantMessageReceived(object sender, MessageSentEventArgs e)
		{
			//IM取得
			InstantMessageModality instantMessageModality = sender as Microsoft.Lync.Model.Conversation.InstantMessageModality;
			//送信者の名前を取得
			var senderName = instantMessageModality.Participant.Contact.GetContactInformation(ContactInformationType.DisplayName);
			//画面に表示
			Console.WriteLine(senderName + ":" + e.Text);
		}

		static ConversationWindow conversationWindow;

		static void CheckPoint(string text = null, [CallerMemberName]string callmethodName = null)
		{
			Console.WriteLine("{0:HH:mm:ss}[{1}]=>{2}", DateTime.Now, callmethodName, text);
		}

	}
}
