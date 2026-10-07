// De gegevens van de projecten staan bij elkaar in een array.
const projecten = [
  {
    titel: "Panda Restaurant Simulator",
    beschrijving: "In deze restaurantsimulator ontwikkel ik een interactieve applicatie waarin gebruikers kunnen experimenteren met het beheren van een panda-restaurant.",
    status: "In ontwikkeling",
    categorie: "Groepsproject",
    mijnBijdrage: "Ik heb GitHub Kanban-boards aangemaakt met DoR en DoD. Daarnaast ben ik bezig met wireframes."
  },
  {
    titel: "Portfolio website",
    beschrijving: "Mijn persoonlijke website met informatie over mijn opleiding, projecten en leerervaringen.",
    status: "In ontwikkeling",
    categorie: "Individueel project",
    mijnBijdrage: "Ik werk aan de HTML, CSS en JavaScript van deze website voor WPFW."
  }
];

const projectenLijst = document.querySelector("#projecten-lijst");
const categorieFilter = document.querySelector("#categorie-filter");
const sorteerKeuze = document.querySelector("#sorteer-keuze");
const aantalProjecten = document.querySelector("#aantal-projecten");

// Maak een artikel voor een project.
function maakProjectArtikel(project) {
  const artikel = document.createElement("article");

  const titel = document.createElement("h2");
  titel.textContent = project.titel;

  const beschrijving = document.createElement("p");
  beschrijving.textContent = project.beschrijving;

  const status = document.createElement("p");
  status.textContent = "Status: " + project.status;

  const categorie = document.createElement("p");
  categorie.textContent = "Categorie: " + project.categorie;

  const bijdrageTitel = document.createElement("h3");
  bijdrageTitel.textContent = "Mijn bijdrage";

  const bijdrage = document.createElement("p");
  bijdrage.textContent = project.mijnBijdrage;

  artikel.append(titel, beschrijving, status, categorie, bijdrageTitel, bijdrage);
  return artikel;
}

// Vervang de oude lijst door de projecten uit de meegegeven array.
function toonProjecten(projectenData) {
  projectenLijst.textContent = "";
  aantalProjecten.textContent = projectenData.length === 1
    ? "1 project gevonden."
    : projectenData.length + " projecten gevonden.";

  if (projectenData.length === 0) {
    const melding = document.createElement("p");
    melding.textContent = "Geen projecten gevonden voor deze categorie.";
    projectenLijst.append(melding);
    return;
  }

  projectenData.forEach((project) => {
    const artikel = maakProjectArtikel(project);
    projectenLijst.append(artikel);
  });
}


// Filter maakt een nieuwe array: de oorspronkelijke projecten blijven behouden.
function filterProjecten(projectenData, categorie) {
  return projectenData.filter((project) => {
    return categorie === "alle" || project.categorie === categorie;
  });
}

function sorteerProjecten(projectenData, volgorde) {
  // spread operator maakt een kopie van de array, zodat de oorspronkelijke volgorde niet verandert.
  const gesorteerd = [...projectenData];
  gesorteerd.sort((a, b) => {
    return a.titel.localeCompare(b.titel, "nl");
  });

  if (volgorde === "za") {
    gesorteerd.reverse();
  }

  return gesorteerd;
}

// Pas beide keuzes samen toe en toon daarna de nieuwe lijst.
function werkProjectenBij() {
  const gefilterd = filterProjecten(projecten, categorieFilter.value);
  const gesorteerd = sorteerProjecten(gefilterd, sorteerKeuze.value);
  toonProjecten(gesorteerd);
}

categorieFilter.addEventListener("change", werkProjectenBij);
sorteerKeuze.addEventListener("change", werkProjectenBij);

werkProjectenBij();