﻿using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using L10NSharp.Translators;

namespace L10NSharp.UI
{
	public partial class LanguageChoosingDialog : Form
	{
		private readonly CultureInfo _requestedCulture;
		private string _originalMessageTemplate;

		public LanguageChoosingDialog(CultureInfo requestedCulture, Icon icon)
		{
			_requestedCulture = requestedCulture;
			InitializeComponent();
			this.Icon = icon;
			_originalMessageTemplate = _messageLabel.Text;
			_messageLabel.Text = string.Format(_originalMessageTemplate, requestedCulture.EnglishName, requestedCulture.NativeName);
			Application.Idle += new EventHandler(Application_Idle);
		}

		void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= new EventHandler(Application_Idle);
			var translator = new BingTranslator("en", _requestedCulture.TwoLetterISOLanguageName);
			try
			{
				var s = translator.TranslateText(string.Format(_originalMessageTemplate, _requestedCulture.EnglishName, _requestedCulture.NativeName));
				if (!string.IsNullOrEmpty(s))
				{
					_messageLabel.Text = s;
					// In general, we will be able to translate OK and the title bar text iff we were able to translate
					// the message.  This assumption saves a few processor cycles and prevents disappearing text when
					// a language has not been localized (as is likely the case when we display this dialog).
					_OKButton.Text = translator.TranslateText("OK");
					Text = translator.TranslateText(Text);
				}
			}
			catch (Exception)
			{
				//swallow
			}
		}

		public string SelectedLanguage;

		private void _OKButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			SelectedLanguage = uiLanguageComboBox1.SelectedLanguage;
			base.OnClosing(e);
		}
	}
}
