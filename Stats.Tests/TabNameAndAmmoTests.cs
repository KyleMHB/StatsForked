using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using RimWorld;
using Stats.ColumnWorkers.ThingDef.RangedWeapon;
using Verse;

namespace Stats.Tests;

[TestFixture]
public sealed class TabNameAndAmmoTests
{
    [Test]
    public void Tab_names_keep_their_duplicate_table_associations_when_an_invalid_tab_is_removed()
    {
        StatsSettings settings = new()
        {
            openTableDefNames = ["RangedWeapons", null!, "RangedWeapons"],
            openTableNames = ["Researched Ranged Weapons", "Removed", "Unresearched"],
            activeTableIndex = 2
        };
        settings.Normalize();
        Assert.That(settings.openTableDefNames, Is.EqualTo(new[] { "RangedWeapons", "RangedWeapons" }));
        Assert.That(settings.openTableNames, Is.EqualTo(new[] { "Researched Ranged Weapons", "Unresearched" }));
        Assert.That(settings.activeTableIndex, Is.EqualTo(1));
    }

    [Test]
    public void Old_settings_without_tab_names_use_original_labels()
    {
        StatsSettings settings = new()
        {
            openTableDefNames = ["RangedWeapons", "Apparel"],
            openTableNames = null!,
            activeTableIndex = 0
        };
        settings.Normalize();
        Assert.That(settings.openTableNames, Is.EqualTo(new[] { "", "" }));
        Assert.That(settings.activeTableIndex, Is.EqualTo(0));
    }

    [Test]
    public void Empty_tab_settings_discard_stale_names_and_selection()
    {
        StatsSettings settings = new() { openTableNames = ["Stale"], activeTableIndex = 4 };
        settings.Normalize();
        Assert.That(settings.openTableNames, Is.Empty);
        Assert.That(settings.activeTableIndex, Is.EqualTo(-1));
    }

    [TestCase(null, "")]
    [TestCase("   ", "")]
    [TestCase("  Researched Ranged Weapons  ", "Researched Ranged Weapons")]
    [TestCase("Weapons\r\nResearched", "Weapons  Researched")]
    public void Tab_name_input_is_normalized(string? input, string expected)
    {
        Assert.That(TableName.Normalize(input), Is.EqualTo(expected));
    }

    [Test]
    public void Tab_names_are_limited_to_the_dialog_length()
    {
        Assert.That(TableName.Normalize(new string('a', 200)).Length, Is.EqualTo(TableName.MaxLength));
    }

    [Test]
    public void Ammo_columns_read_configured_reloadable_properties_without_guessing_from_weapon_tags()
    {
        ThingDef ammo = MakeThingDef();
        ammo.defName = "CustomAmmo";
        ammo.label = "custom ammo";
        CompProperties_ApparelReloadable properties = new()
        {
            maxCharges = 37,
            ammoCountPerCharge = 4,
            ammoDef = ammo
        };
        ThingDef weapon = MakeThingDef();
        weapon.comps = new List<CompProperties> { new CompProperties(), properties };
        weapon.weaponTags = ["ammoDef_wrongAmmo/99/9"];
        CompProperties_ApparelReloadable? actual = YayoAmmo.GetProperties(weapon);
        Assert.That(actual, Is.SameAs(properties));
        Assert.That(actual!.maxCharges, Is.EqualTo(37));
        Assert.That(actual.ammoCountPerCharge, Is.EqualTo(4));
        Assert.That(actual.ammoDef, Is.SameAs(ammo));
    }

    [Test]
    public void Weapons_without_an_ammo_component_have_no_ammo_data()
    {
        Assert.That(YayoAmmo.GetProperties(MakeThingDef()), Is.Null);
        Assert.That(YayoAmmo.GetProperties(null), Is.Null);
    }

    private static ThingDef MakeThingDef()
    {
        // The definition constructor loads Unity shaders; these tests only need component data.
        ThingDef def = (ThingDef)FormatterServices.GetUninitializedObject(typeof(ThingDef));
        def.comps = [];
        return def;
    }

}
