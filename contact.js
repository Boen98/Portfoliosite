const contactFormulier = document.querySelector("#contact-formulier");
const contactVelden = document.querySelector("#contact-velden");
const contactStatus = document.querySelector("#contact-status");
const velden = [
  document.querySelector("#naam"),
  document.querySelector("#email"),
  document.querySelector("#bericht")
];
let formulierGecontroleerd = false;

// Geef een begrijpelijke foutmelding terug, of een lege tekst bij geldige invoer.
function bepaalFoutmelding(veld) {
  const waarde = veld.value.trim();

  if (veld.id === "naam" && waarde === "") {
    return "Vul je naam in.";
  }

  if (veld.id === "email") {
    if (waarde === "") {
      return "Vul je e-mailadres in.";
    }
    if (veld.validity.typeMismatch) {
      return "Vul een geldig e-mailadres in, bijvoorbeeld naam@voorbeeld.nl.";
    }
  }

  if (veld.id === "bericht" && waarde.length < 10) {
    return "Schrijf een bericht van minimaal 10 tekens.";
  }

  return "";
}

// Werk de melding en de toegankelijkheidsinformatie van een veld bij.
function controleerVeld(veld) {
  const foutmelding = bepaalFoutmelding(veld);
  const foutElement = document.querySelector("#" + veld.id + "-fout");
  foutElement.textContent = foutmelding;

  if (foutmelding === "") {
    veld.setAttribute("aria-invalid", "false");
    return true;
  } else {
    veld.setAttribute("aria-invalid", "true");
    return false;
  }
}

function verwerkContactformulier(event) {
  // Dit formulier controleert alleen invoer; er wordt niets verzonden.
  event.preventDefault();
  formulierGecontroleerd = true;
  contactStatus.textContent = "";
  let eersteOngeldigeVeld = null;

  velden.forEach((veld) => {
    const geldig = controleerVeld(veld);
    if (!geldig && eersteOngeldigeVeld === null) {
      eersteOngeldigeVeld = veld;
    }
  });

  if (eersteOngeldigeVeld !== null) {
    eersteOngeldigeVeld.focus();
    return;
  }

  contactStatus.textContent = "Bedankt! Je gegevens zijn geldig. Dit is een oefenformulier; je bericht is niet verzonden.";
}

contactFormulier.addEventListener("submit", verwerkContactformulier);

velden.forEach((veld) => {
  veld.addEventListener("input", () => {
    // Een eerdere bevestiging geldt niet meer zodra de invoer verandert.
    contactStatus.textContent = "";
    if (formulierGecontroleerd) {
      controleerVeld(veld);
    }
  });
});

// Gebruik de eigen foutmeldingen. Activeer het formulier pas na het instellen van de events.
contactFormulier.noValidate = true;
contactVelden.disabled = false;
document.querySelector("#contact-zonder-js").hidden = true;
