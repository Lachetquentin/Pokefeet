using Microsoft.JSInterop;
using MudBlazor;
using Pokefeet2.Class;

namespace Pokefeet2.Pages;

partial class Index
{
	readonly DialogOptions _dialogOptions = new() { FullWidth = true };
	IJSObjectReference? _jsRef;

	bool _visible;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_jsRef = await Js.InvokeAsync<IJSObjectReference>(Constants.Javascript.Import, Constants.Javascript.ImportPath);

			if (_jsRef != null)
			{
				await _jsRef.InvokeVoidAsync(Constants.Javascript.CheckLastPlayedDate);
				await _jsRef.InvokeVoidAsync(Constants.Javascript.CheckHasWinClassic);
				await _jsRef.InvokeVoidAsync(Constants.Javascript.CheckPokemonGuesses);
				await _jsRef.InvokeVoidAsync(Constants.Javascript.CheckDaily);
			}
		}
	}

	void Close() => _visible = false;
	void OpenDialog() => _visible = true;
	void StartDaily() => Navigation.NavigateTo(Constants.Url.Daily);
	void StartInfinite() => Navigation.NavigateTo(Constants.Url.Infinite);
}
