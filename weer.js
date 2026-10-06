// Een vaste locatie: Den Haag. Er is geen API-key of locatie-toestemming nodig.
// Documentatie: https://open-meteo.com/en/docs
const weerUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.0705&longitude=4.3007&current=temperature_2m,wind_speed_10m&temperature_unit=celsius&wind_speed_unit=kmh&timeformat=unixtime&timezone=Europe%2FAmsterdam&forecast_days=1";
const weerStatus = document.querySelector("#weer-status");
const weerGegevens = document.querySelector("#weer-gegevens");
const weerKnop = document.querySelector("#weer-vernieuwen");

// Haal JSON op en controleer of de verwachte gegevens aanwezig zijn.
async function haalWeerOp() {
  const response = await fetch(weerUrl, {
    signal: AbortSignal.timeout(10000)
  });

  // Ook bij bijvoorbeeld HTTP 404 of 500 moet de fout worden afgehandeld.
  if (!response.ok) {
    throw new Error("Het weer kon niet worden opgehaald.");
  }

  const data = await response.json();
  const huidig = data.current;

  if (!huidig || !Number.isFinite(huidig.temperature_2m)
      || !Number.isFinite(huidig.wind_speed_10m)
      || !Number.isFinite(huidig.time)
      || Number.isNaN(new Date(huidig.time * 1000).getTime())) {
    throw new Error("De weergegevens zijn onvolledig.");
  }

  return huidig;
}

// Zet de gegevens uit de API om in zichtbare HTML-elementen.
function toonWeer(huidig) {
  weerGegevens.textContent = "";

  const temperatuur = document.createElement("p");
  temperatuur.textContent = "Temperatuur: " + huidig.temperature_2m.toLocaleString("nl-NL") + " °C";

  const wind = document.createElement("p");
  wind.textContent = "Windsnelheid: " + huidig.wind_speed_10m.toLocaleString("nl-NL") + " km/u";

  // De API geeft seconden sinds 1970; Date gebruikt milliseconden.
  const tijd = new Date(huidig.time * 1000);
  const tijdstip = document.createElement("p");
  tijdstip.textContent = "Gegevens voor: " + tijd.toLocaleString("nl-NL", {
    timeZone: "Europe/Amsterdam",
    dateStyle: "short",
    timeStyle: "short"
  }) + " (tijd in Den Haag).";

  weerGegevens.append(temperatuur, wind, tijdstip);
}

// Beheer de laadstatus, foutmelding en vernieuwknop.
async function laadWeer() {
  weerKnop.disabled = true;
  weerStatus.textContent = "Het weer wordt geladen…";
  weerStatus.classList.remove("weer-fout");
  weerGegevens.textContent = "";
  weerGegevens.setAttribute("aria-busy", "true");

  try {
    const huidig = await haalWeerOp();
    toonWeer(huidig);
    weerStatus.textContent = "Het weer is geladen.";
    weerKnop.textContent = "Weer vernieuwen";
  } catch (fout) {
    weerStatus.textContent = "Het weer kon niet worden geladen. Controleer je internetverbinding of probeer het later opnieuw.";
    weerStatus.classList.add("weer-fout");
    weerKnop.textContent = "Opnieuw proberen";
  } finally {
    // Ook na een fout moet de bezoeker het opnieuw kunnen proberen.
    weerKnop.disabled = false;
    weerGegevens.setAttribute("aria-busy", "false");
  }
}

weerKnop.addEventListener("click", laadWeer);
laadWeer();