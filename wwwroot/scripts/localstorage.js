const currentDate = new Date().toLocaleDateString();
var lastPlayedDate = localStorage.getItem('lastPlayedDate');

export function addPokemonGuess(newGuess) {
  var jsonPokemonGuesses = localStorage.getItem("pokemonGuesses");
  var pokemonGuesses = jsonPokemonGuesses ? JSON.parse(jsonPokemonGuesses) : [];

  pokemonGuesses.push(newGuess);

  var updatedJsonPokemonGuesses = JSON.stringify(pokemonGuesses);

  localStorage.setItem("pokemonGuesses", updatedJsonPokemonGuesses);
}

export function checkLastPlayedDate() {
    if (!localStorage.getItem('lastPlayedDate')) {
        localStorage.setItem('lastPlayedDate', currentDate);
    }
}

export function checkHasWinClassic() {
    if (!localStorage.getItem('hasWinClassic')) {
        localStorage.setItem('hasWinClassic', '0');
    }
}

export function checkPokemonGuesses() {
     if (!localStorage.getItem('pokemonGuesses')) {
        localStorage.setItem('pokemonGuesses', []);
    }
}

export function checkDaily() {
    if (lastPlayedDate !== currentDate) {
        localStorage.setItem('hasWinClassic', '0');
        localStorage.setItem('lastPlayedDate', currentDate);
        localStorage.setItem('pokemonGuesses', []);
        lastPlayedDate = currentDate;
    }
}

export function saveLocal(key, value) {
    localStorage.setItem(key, value);
}

export function getLocal(key) {
    return localStorage.getItem(key);
}

export function getLocalArray(key) {
    var item = localStorage.getItem(key);

    if (item === null || item === undefined) {
        return null;    
    }

    return JSON.stringify(item);
}