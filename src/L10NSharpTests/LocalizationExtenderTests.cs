﻿using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using L10NSharp.UI;
using NUnit.Framework;

namespace L10NSharp.Tests
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class LocalizationExtenderTests
	{
		private LocalizationExtender m_extender;
		private Dictionary<object, LocalizingInfo> m_extCtrls;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Setup for each test.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SetUp]
		public void TestSetup()
		{
			m_extender = new LocalizationExtender();
			m_extCtrls = ReflectionHelper.GetField(m_extender, "m_extendedCtrls") as
				Dictionary<object, LocalizingInfo>;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the get and set properties of the extender.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetSetTests()
		{
			Assert.AreEqual(0, m_extCtrls.Count);

			var lbl = new Label();
			m_extender.SetLocalizableToolTip(lbl, "lions");
			m_extender.SetLocalizationComment(lbl, "tigers");
			m_extender.SetLocalizingId(lbl, "bears");
			m_extender.SetLocalizationPriority(lbl, LocalizationPriority.MediumLow);

			Assert.AreEqual(1, m_extCtrls.Count);
			Assert.AreEqual("lions", m_extender.GetLocalizableToolTip(lbl));
			Assert.AreEqual("tigers", m_extender.GetLocalizationComment(lbl));
			Assert.AreEqual("bears", m_extender.GetLocalizingId(lbl));
			Assert.AreEqual(LocalizationPriority.MediumLow, m_extender.GetLocalizationPriority(lbl));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the GetLocalizedObjectInfo method.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GetLocalizedObjectInfoTest()
		{
			Assert.AreEqual(0, m_extCtrls.Count);

			var lbl1 = new Label();
			lbl1.Text = "bananas";

			// Make sure calling GetLocalizedObjectInfo creates a LocalizingInfo object when
			// one doesn't exist for the label.
			var loi = ReflectionHelper.GetResult(m_extender, "GetLocalizedObjectInfo", lbl1) as LocalizingInfo;
			Assert.AreEqual(1, m_extCtrls.Count);
			Assert.AreEqual("bananas", loi.Text);

			// Make sure calling GetLocalizedObjectInfo does not create a LocalizingInfo object when
			// one already exists for the label.
			loi = ReflectionHelper.GetResult(m_extender, "GetLocalizedObjectInfo", lbl1) as LocalizingInfo;
			Assert.AreEqual(1, m_extCtrls.Count);
			Assert.AreEqual("bananas", loi.Text);

			// Create a new LocalizingInfo object for a different label, then poke it into the
			// extender's internal collection and make sure calling GetLocalizedObjectInfo returns
			// that LocalizingInfo for the object.
			var lbl2 = new Label();
			lbl2.Text = "apples";
			loi = new LocalizingInfo(lbl2);
			m_extCtrls[lbl2] = loi;
			Assert.AreEqual(2, m_extCtrls.Count);

			loi = ReflectionHelper.GetResult(m_extender, "GetLocalizedObjectInfo", lbl2) as LocalizingInfo;
			Assert.AreEqual(2, m_extCtrls.Count);
			Assert.AreEqual("apples", loi.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the FinalizationForListViewColumnHeaders method.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test, Ignore("I don't see code around that would make this work(e.g. insert a 'Col'), maybe David left it as a todo?")]
		public void PrepareListViewColumnHeadersTests()
		{
			Form frm = new Form();
			frm.Name = "TestDlg";

			var lv = new ListView();
			lv.Name = "meat";
			lv.Columns.Add("ham", "hamtext");
			lv.Columns.Add("steak", "steaktext");
			lv.Columns.Add("venison", "venisontext");
			frm.Controls.Add(lv);

			ReflectionHelper.CallMethod(m_extender, "GetLocalizedObjectInfo", lv);
			Assert.AreEqual(1, m_extCtrls.Count);
			Assert.IsTrue(m_extCtrls.ContainsKey(lv));

			ReflectionHelper.CallMethod(m_extender, "FinalizationForListViewColumnHeaders", null);

			Assert.AreEqual(3, m_extCtrls.Count);
			Assert.IsTrue(m_extCtrls.ContainsKey(lv.Columns["ham"]));
			Assert.IsTrue(m_extCtrls.ContainsKey(lv.Columns["steak"]));
			Assert.IsTrue(m_extCtrls.ContainsKey(lv.Columns["venison"]));

			LocalizingInfo loi = m_extCtrls[lv.Columns["ham"]];
			Assert.AreEqual("TestDlg.meatColham", loi.Id);
			Assert.AreEqual("hamtext", loi.Text);

			loi = m_extCtrls[lv.Columns["steak"]];
			Assert.AreEqual("TestDlg.meatColsteak", loi.Id);
			Assert.AreEqual("steaktext", loi.Text);

			loi = m_extCtrls[lv.Columns["venison"]];
			Assert.AreEqual("TestDlg.meatColvenison", loi.Id);
			Assert.AreEqual("venisontext", loi.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests the FinalizationForDataGridViewColumns method.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test, Ignore("I don't see code around that would make this work(e.g. insert a 'Col'), maybe David left it as a todo?")]
		public void PrepareDataGridViewColumnsTests()
		{
			Form frm = new Form();
			frm.Name = "TestDlg";

			DataGridView grid = new DataGridView();
			grid.Name = "colors";
			grid.Columns.Add("red", "redtext");
			grid.Columns.Add("blue", "bluetext");
			grid.Columns.Add("orange", "orangetext");
			frm.Controls.Add(grid);

			ReflectionHelper.CallMethod(m_extender, "GetLocalizedObjectInfo", grid);
			Assert.AreEqual(1, m_extCtrls.Count);
			Assert.IsTrue(m_extCtrls.ContainsKey(grid));

			ReflectionHelper.CallMethod(m_extender, "FinalizationForDataGridViewColumns", null);

			Assert.AreEqual(3, m_extCtrls.Count);
			Assert.IsTrue(m_extCtrls.ContainsKey(grid.Columns["red"]));
			Assert.IsTrue(m_extCtrls.ContainsKey(grid.Columns["blue"]));
			Assert.IsTrue(m_extCtrls.ContainsKey(grid.Columns["orange"]));

			LocalizingInfo loi = m_extCtrls[grid.Columns["red"]];
			Assert.AreEqual("TestDlg.colorsColred", loi.Id);
			Assert.AreEqual("redtext", loi.Text);

			loi = m_extCtrls[grid.Columns["blue"]];
			Assert.AreEqual("TestDlg.colorsColblue", loi.Id);
			Assert.AreEqual("bluetext", loi.Text);

			loi = m_extCtrls[grid.Columns["orange"]];
			Assert.AreEqual("TestDlg.colorsColorange", loi.Id);
			Assert.AreEqual("orangetext", loi.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the EndInit method goes through the extended controls and sets the
		/// group for each.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void GroupAssignementTest()
		{
			//LocalizationManager.StringFilesFolder = Path.GetPathRoot(Path.GetTempPath());
			//LocalizationManager.Enabled = true;

			//Assert.AreEqual(0, m_extCtrls.Count);

			//m_extCtrls[0] = new LocalizingInfo("GrpName.trout");
			//m_extCtrls[1] = new LocalizingInfo("GrpName.pike");
			//m_extCtrls[2] = new LocalizingInfo("GrpName.catfish");

			//Assert.AreNotEqual("GrpName", m_extCtrls[0].Group);
			//Assert.AreNotEqual("GrpName", m_extCtrls[1].Group);
			//Assert.AreNotEqual("GrpName", m_extCtrls[2].Group);

			//m_extender.EndInit();

			//Assert.AreEqual("GrpName", m_extCtrls[0].Group);
			//Assert.AreEqual("GrpName", m_extCtrls[1].Group);
			//Assert.AreEqual("GrpName", m_extCtrls[2].Group);
		}
	}
}
