﻿using System;
using System.Windows.Forms;

namespace Localization.UI
{
	/// ----------------------------------------------------------------------------------------
	internal class ShortcutKeysDropDown : CustomDropDownComboBox
	{
		private readonly ShortcutKeysEditor m_editor;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ShortcutKeysDropDown"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public ShortcutKeysDropDown()
		{
			AlignDropToLeft = false;
			TextBox.ReadOnly = true;
			TextBox.KeyDown += HandleTextBoxKeyDown;
			m_editor = new ShortcutKeysEditor();
			PopupCtrl = m_editor;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the shortcut keys drop down closed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnPopupClosed(object sender, EventArgs e)
		{
			base.OnPopupClosed(sender, e);
			SetCurrentKeysFromEditor();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the shortcut keys.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Keys ShortcutKeys
		{
			get { return m_editor.SelectedKeys; }
			set
			{
				m_editor.SelectedKeys = value;
				SetCurrentKeysFromEditor();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the shortcut keys as string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string ShortcutKeysAsString
		{
			get { return m_editor.SelectedKeysAsString; }
			set
			{
				m_editor.SelectedKeysAsString = value;
				SetCurrentKeysFromEditor();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the current keys.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetCurrentKeysFromEditor()
		{
			Text = (m_editor.SelectedKeys == Keys.None ? string.Empty : m_editor.SelectedKeysAsString);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clears the shortcut keys when the user presses backspace or delete in the text
		/// box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
			{
				m_editor.Reset();
				SetCurrentKeysFromEditor();
			}
		}
	}
}
