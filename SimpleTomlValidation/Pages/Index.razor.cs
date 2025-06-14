using BlazorMonaco;
using BlazorMonaco.Editor;
using CsToml;
using CsToml.Error;
using Microsoft.AspNetCore.Components;
using SimpleTomlValidation.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SimpleTomlValidation.Pages;

public partial class Index
{
    [AllowNull]
    private StandaloneCodeEditor editor;

    private bool isValid = true;
    private string selectedSpec = "v1.0.0";
    private bool allowUnicodeInBareKeys = false;
    private bool allowNewlinesInInlineTables = false;
    private bool allowTrailingCommaInInlineTables = false;
    private bool allowSecondsOmissionInTime = false;
    private bool supportsEscapeSequenceE = false;
    private bool supportsEscapeSequenceX = false;
    private bool isFeaturesExpanded = true;

    public string TomlStatus => isValid ? "This TOML text is normal." : "Error !";

    public string ValidationStatusStyle => isValid ? "toml-valid" : "toml-invalid";

    private static StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            Language = "toml",
            AutomaticLayout = true,
            GlyphMargin = true,
            Value = @$"
# This is a TOML document

title = ""TOML Example""

[owner]
name = ""Tom Preston-Werner""
dob = 1979-05-27T07:32:00-08:00

[database]
enabled = true
ports = [ 8000, 8001, 8002 ]
data = [ [""delta"", ""phi""], [3.14] ]
temp_targets = {{ cpu = 79.5, case = 72.0 }}

[servers]

[servers.alpha]
ip = ""10.0.0.1""
role = ""frontend""

[servers.beta]
ip = ""10.0.0.2""
role = ""backend""
"
        };
    }

    private async Task ChangeTheme(ChangeEventArgs e)
    {
        await Global.SetTheme(jsRuntime, e.Value?.ToString());
    }

    private void ChangeSpec(ChangeEventArgs e)
    {
        selectedSpec = e.Value?.ToString() ?? "v1.0.0";
    }

    private void ToggleFeaturesExpanded()
    {
        isFeaturesExpanded = !isFeaturesExpanded;
    }

    private async Task OnClickValidate()
    {
        try
        {
            await editor.ResetDeltaDecorations();
            
            var utf16Value = await editor.GetValue();
            
            var options = CreateSerializerOptions();
            var doc = CsTomlSerializer.Deserialize<TomlDocument>(Encoding.UTF8.GetBytes(utf16Value), options);

            isValid = true;
            StateHasChanged();
        }
        catch (OperationCanceledException)
        {
            // Validation was cancelled, do nothing
            return;
        }
        catch (CsTomlSerializeException ctse)
        {
            isValid = false;

            var errorDecorations = new ExtendableArray<ModelDeltaDecoration>(ctse.ParseExceptions!.Count);
            try
            {
                foreach (var e in ctse.ParseExceptions!)
                {
                    errorDecorations.Add(new()
                    {
                        Range = new BlazorMonaco.Range((int)e.LineNumber, 1, (int)e.LineNumber, 1),
                        Options = new ModelDecorationOptions
                        {
                            IsWholeLine = true,
                            ClassName = "decorationContentClass",
                            GlyphMarginClassName = "decorationGlyphMarginClass"
                        }
                    });
                }

                await editor.DeltaDecorations(null, errorDecorations.AsSpan().ToArray());
            }
            finally
            {
                errorDecorations.Return();
            }

            StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            isValid = false;
            StateHasChanged();
        }
    }

    private async Task OnClickClear()
    {
        isValid = true;
        await editor.SetValue(string.Empty);
    }

    private CsTomlSerializerOptions CreateSerializerOptions()
    {
        if (selectedSpec == "v1.0.0")
        {
            return CsTomlSerializerOptions.Default;
        }

        return CsTomlSerializerOptions.Default with
        {
            Spec = new()
            {
                AllowUnicodeInBareKeys = allowUnicodeInBareKeys,
                AllowNewlinesInInlineTables = allowNewlinesInInlineTables,
                AllowTrailingCommaInInlineTables = allowTrailingCommaInInlineTables,
                AllowSecondsOmissionInTime = allowSecondsOmissionInTime,
                SupportsEscapeSequenceE = supportsEscapeSequenceE,
                SupportsEscapeSequenceX = supportsEscapeSequenceX
            }
        };
    }

}