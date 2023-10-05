using Microsoft.JSInterop;
using MudBlazor;

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
			_jsRef = await Js.InvokeAsync<IJSObjectReference>("import", "./scripts/localstorage.js");

			if (_jsRef != null)
			{
				await _jsRef.InvokeVoidAsync("checkLastPlayedDate");
				await _jsRef.InvokeVoidAsync("checkHasWinClassic");
				await _jsRef.InvokeVoidAsync("checkPokemonGuesses");
				await _jsRef.InvokeVoidAsync("checkDaily");
			}
		}
	}

	void Close() => _visible = false;
	void OpenDialog() => _visible = true;
	void StartDaily() => Navigation.NavigateTo("/daily");
}
