﻿using System;
using Localization.TMXUtils;

namespace Localization
{
	internal class TransUnitUpdater
	{
		internal const string kToolTipSuffix = "_ToolTip_";
		internal const string kShortcutSuffix = "_ShortcutKeys_";

		// Cannot use Environment.NewLine because that also includes a carriage return
		// character which, when included, messes up the display of text in controls.
		internal const string kOSRealNewline = "\n";

		// This is the symbol for a newline that users put in their localized text when
		// they want a real newline inserted. The program will replace literal newlines
		// with the value of kOSNewline.
		internal string _literalNewline = "\\n";

		private readonly TMXDocument _tmxDoc;
		private bool _updated;

		/// ------------------------------------------------------------------------------------
		internal TransUnitUpdater(TMXDocument tmxDoc)
		{
			_tmxDoc = tmxDoc;

			var replacement = _tmxDoc.Header.GetPropValue("x-hardlinebreakreplacement");
			if (replacement != null)
				_literalNewline = replacement;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the localized info. in the cache with the info. from the specified
		/// LocalizedObjectInfo.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		internal bool Update(TMXDocument tmxDoc, LocalizingInfo locInfo)
		{
			_updated = false;

			// Can't do anything without a language id.
			if (string.IsNullOrEmpty(locInfo.LangId))
				return _updated;

			var tuText = _tmxDoc.GetTransUnitForId(locInfo.Id);
			var tuToolTip = _tmxDoc.GetTransUnitForId(locInfo.Id + kToolTipSuffix);
			var tuShortcutKeys = _tmxDoc.GetTransUnitForId(locInfo.Id + kShortcutSuffix);

			if (locInfo.Priority == LocalizationPriority.NotLocalizable)
			{
				_updated = (tuText != null || tuToolTip != null || tuShortcutKeys != null);
				_tmxDoc.RemoveTransUnit(tuText);
				_tmxDoc.RemoveTransUnit(tuToolTip);
				_tmxDoc.RemoveTransUnit(tuShortcutKeys);
				return _updated;
			}

			// Save the shortcut keys
			if ((locInfo.UpdateFields & UpdateFields.ShortcutKeys) == UpdateFields.ShortcutKeys)
				tuShortcutKeys = UpdateValue(tuShortcutKeys, locInfo.ShortcutKeys, locInfo, locInfo.Id + kShortcutSuffix);

			// Save the tooltips
			if ((locInfo.UpdateFields & UpdateFields.ToolTip) == UpdateFields.ToolTip)
				tuToolTip = UpdateValue(tuToolTip, locInfo.ToolTipText, locInfo, locInfo.Id + kToolTipSuffix);

			// Save the text
			if ((locInfo.UpdateFields & UpdateFields.Text) == UpdateFields.Text)
			{
				var text = locInfo.Text ?? string.Empty;
				text = text.Replace(Environment.NewLine, _literalNewline);
				text = text.Replace(_literalNewline, "@#$");
				text = text.Replace(kOSRealNewline, _literalNewline);
				text = text.Replace("@#$", _literalNewline);
				tuText = UpdateValue(tuText, text, locInfo, locInfo.Id);
			}

			if ((locInfo.UpdateFields & UpdateFields.Comment) != UpdateFields.Comment)
				return _updated;

			// Save the comments.
			if (tuText != null)
			{
				tuText.Notes.Clear();
				if (!string.IsNullOrEmpty(locInfo.Comment))
					tuText.AddNote(locInfo.Comment);
				return _updated;
			}

			if (tuToolTip != null)
			{
				tuToolTip.Notes.Clear();
				if (!string.IsNullOrEmpty(locInfo.Comment))
					tuToolTip.AddNote(locInfo.Comment);
				return _updated;
			}

			if (tuShortcutKeys != null)
			{
				tuShortcutKeys.Notes.Clear();
				if (!string.IsNullOrEmpty(locInfo.Comment))
					tuShortcutKeys.AddNote(locInfo.Comment);
			}

			return _updated;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the value for the specified translation unit with the specified new value.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private TransUnit UpdateValue(TransUnit tu, string newValue, LocalizingInfo locInfo, string tuId)
		{
			newValue = newValue ?? string.Empty;

			if (tu != null)
			{
				var tuv = tu.GetVariantForLang(locInfo.LangId);
				if (tuv != null)
				{
					_updated = true;
					tu.RemoveVariant(tuv);
					if (tu.Variants.Count == 0)
						_tmxDoc.RemoveTransUnit(tu);
				}
			}

			if (newValue == string.Empty)
				return tu;

			// Create a new entry if needed.
			if (tu == null)
			{
				tu = new TransUnit();
				tu.Id = tuId;
				_tmxDoc.AddTransUnit(tu);
			}

			tu.AddVariant(locInfo.LangId, newValue);
			_updated = true;
			return tu;
		}
	}
}
