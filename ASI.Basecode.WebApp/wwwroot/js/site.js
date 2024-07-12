let dpicn = document.querySelector(".dpicn");
let dropdown = document.querySelector(".dropdown");

dpicn.addEventListener("click", () => {
    console.log('open');
    dropdown.classList.toggle("dropdown-open");
})