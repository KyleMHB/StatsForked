using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public sealed class Thing_RecipesColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Texture2D IngredientTexture;
    public Thing_RecipesColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    static Thing_RecipesColumnWorker()
    {
        IngredientTexture = ContentFinder<Texture2D>.Get("UI/Icons/ThingCategories/ResourcesRaw");
    }
    // TODO: Handle same single ThingDef ingredients.
    private static IEnumerable<(ThingDef[] ThingDefs, int Count)> GetThingDefRecipeIngredients(RecipeDef recipeDef, ThingDef? stuffDef)
    {
        foreach (var ingredient in recipeDef.ingredients)
        {
            var ingredientAllowedThingDefsCount = ingredient.filter.AllowedDefCount;

            if (ingredientAllowedThingDefsCount == 1)
            {
                var ingredientThingDef = ingredient.filter.AnyAllowedDef;
                var ingredientCount = ingredient.CountRequiredOfFor(ingredientThingDef, recipeDef);

                yield return ([ingredientThingDef], ingredientCount);
            }
            else if (ingredientAllowedThingDefsCount > 1)
            {
                if (stuffDef != null && ingredient.filter.Allows(stuffDef))
                {
                    var ingredientThingDef = stuffDef;
                    var ingredientCount = ingredient.CountRequiredOfFor(stuffDef, recipeDef);

                    yield return ([ingredientThingDef], ingredientCount);
                }
                else
                {
                    yield return (ingredient.filter.AllowedThingDefs.ToArray(), (int)ingredient.GetBaseCount());
                }
            }
        }
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var recipeDefs = thing.Def.GetRecipeDefs();

        if (recipeDefs == null || recipeDefs.Count == 0)
        {
            return null;
        }

        var recipeWidgets = new List<Widget>();

        foreach (var recipeDef in recipeDefs)
        {
            Widget recipeWidget = RecipeDefToWidget(recipeDef, thing).WidthRel(1f);

            if (recipeWidgets.Count != 0)
            {
                recipeWidget = recipeWidget
                    .PaddingAbs(0f, 0f, Globals.GUI.PadSm, 0f)
                    .BorderTop(Widgets.MainTabWindow.BorderLineColor)
                    .PaddingAbs(0f, 0f, Globals.GUI.PadSm, 0f);
            }

            recipeWidgets.Add(recipeWidget);
        }

        return new VerticalContainer(recipeWidgets);
    }
    private static Widget RecipeDefToWidget(RecipeDef recipeDef, ThingAlike thing)
    {
        var rows = new List<Widget>();

        // Product amount
        var productAmount = 0;

        foreach (var product in recipeDef.products)
        {
            if (product.thingDef == thing.Def)
            {
                productAmount = product.count;
            }
        }

        if (productAmount > 1)
        {
            var row = new Label("Products: ".Colorize(Globals.GUI.TextHighlightColor) + productAmount.ToString());

            rows.Add(row);
        }

        // Ingredients
        if (recipeDef.ingredients.Count > 0)
        {
            var ingredientWidgets = new List<Widget>(recipeDef.ingredients.Count);

            foreach (var (thingDefs, count) in GetThingDefRecipeIngredients(recipeDef, thing.StuffDef))
            {
                Widget icon;

                if (thingDefs.Length == 1)
                {
                    var thingDef = thingDefs[0];

                    icon = new ThingIcon(thingDef).ToButtonGhostly(() => Draw.DefInfoDialog(thingDef), thingDef.LabelCap);
                }
                else
                {
                    var stringBuilder = new StringBuilder();

                    foreach (var thingDef in thingDefs)
                    {
                        var line = $"- {thingDef.LabelCap}";

                        if (thingDef.smallVolume)
                        {
                            line += $" x{ThingDef.SmallUnitPerVolume}".Colorize(Globals.GUI.TextHighlightColor);
                        }

                        stringBuilder.AppendLine(line);
                    }

                    icon = new Icon(IngredientTexture).Tooltip(stringBuilder.ToString());
                }

                var ingredientWidget = new HorizontalContainer([new Label(count.ToString()), icon]);

                ingredientWidgets.Add(ingredientWidget);
            }

            var row = new HorizontalContainer(ingredientWidgets, Globals.GUI.PadSm);

            rows.Add(row);
        }

        // Skills
        var recipeSkillRequirements = recipeDef.skillRequirements;

        if (recipeSkillRequirements?.Count > 0)
        {
            var stringBuilder = new StringBuilder();

            foreach (var skillRequirement in recipeSkillRequirements)
            {
                var skillLabel = skillRequirement.skill.LabelCap;
                var skillMinLevel = skillRequirement.minLevel.ToString();

                if (skillRequirement.skill == recipeDef.workSkill)
                {
                    skillMinLevel = skillMinLevel.Colorize(Globals.GUI.TextHighlightColor);
                }

                stringBuilder.AppendInNewLine($"{skillLabel}: {skillMinLevel}");
            }

            var row = new Label(stringBuilder.ToString());

            rows.Add(row);
        }

        // Work amount
        // TODO: Find where the 60 is defined.
        var workAmount = Mathf.CeilToInt(recipeDef.WorkAmountForStuff(thing.StuffDef) / 60f);

        if (workAmount > 0)
        {
            Widget label = new Label($"Work: {workAmount}");

            if (recipeDef.workSpeedStat != null)
            {
                // TODO: Add more factors. Importantly, stuff-related ones.
                label = label.Tooltip($"Factors:\n- {recipeDef.workSpeedStat.LabelCap}");
            }

            rows.Add(label);
        }

        // Recipe users
        var recipeUsers = recipeDef.GetAllRecipeUsers();

        if (recipeUsers?.Count > 0)
        {
            var recipeUsersIcons = recipeUsers.Select(thingDef => new ThingIcon(thingDef).ToButtonGhostly(() => Draw.DefInfoDialog(thingDef), thingDef.LabelCap));
            var row = new HorizontalContainer([.. recipeUsersIcons], Globals.GUI.PadSm);

            rows.Add(row);
        }

        return new VerticalContainer(rows);
    }
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetAllRecipesUsers =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var allRecipesUsers = new HashSet<ThingDef>();
        var recipeDefs = thing.Def.GetRecipeDefs();

        if (recipeDefs?.Count > 0)
        {
            foreach (var recipeDef in recipeDefs)
            {
                var recipeUsers = recipeDef.GetAllRecipeUsers();

                allRecipesUsers.AddRange(recipeUsers);
            }
        }

        return allRecipesUsers;
    });
    private static readonly Func<ThingAlike, HashSet<SkillDef>> GetAllSkillDefs =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var allSkillDefs = new HashSet<SkillDef>();
        var recipeDefs = thing.Def.GetRecipeDefs();

        if (recipeDefs?.Count > 0)
        {
            foreach (var recipeDef in recipeDefs)
            {
                if (recipeDef.skillRequirements?.Count > 0)
                {
                    allSkillDefs.AddRange(recipeDef.skillRequirements.Select(skillReq => skillReq.skill));
                }
            }
        }

        return allSkillDefs;
    });
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetAllIngredients =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var allIngredients = new HashSet<ThingDef>();
        var recipeDefs = thing.Def.GetRecipeDefs();

        if (recipeDefs?.Count > 0)
        {
            foreach (var recipeDef in recipeDefs)
            {
                var ingredients = GetThingDefRecipeIngredients(recipeDef, thing.StuffDef);

                allIngredients.AddRange(ingredients.SelectMany((ingredient) => ingredient.ThingDefs));
            }
        }

        return allIngredients;
    });
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var recipeUsersFilter = Make.MTMThingDefFilter(GetAllRecipesUsers, tableRecords).Tooltip("Filter by crafting benches.");
        var skillsFilter = Make.MTMDefFilter(GetAllSkillDefs, tableRecords).Tooltip("Filter by skills.");
        var ingredientsFilter = Make.MTMThingDefFilter(GetAllIngredients, tableRecords).Tooltip("Filter by ingredients.");

        return Make.CompositeFilter<ThingAlike>([
            ingredientsFilter,
            recipeUsersFilter,
            skillsFilter,
        ], true);
    }
    private static readonly Func<ThingAlike, float> GetMinWorkAmount =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var recipeDefs = thing.Def.GetRecipeDefs();

        if (recipeDefs?.Count > 0)
        {
            return recipeDefs.Select(recipeDef => recipeDef.WorkAmountForStuff(thing.StuffDef)).Min();
        }

        return 0f;
    });
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetMinWorkAmount(thing1).CompareTo(GetMinWorkAmount(thing2));
    }
}
