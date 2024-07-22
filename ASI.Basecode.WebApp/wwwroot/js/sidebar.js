document.addEventListener("DOMContentLoaded", function () {
    const sidebarItems = document.querySelectorAll('.sidebar a');
    console.log(sidebarItems)
    const setActiveClass = () => {
        const currPath = window.location.pathname;
        sidebarItems.forEach(item => {
            console.log(item.getAttribute('href').split('?')[0])
            if (item.getAttribute('href').split('?')[0] === currPath) {
                item.parentElement.classList.add('active');
            } else {
                item.parentElement.classList.remove('active');
            }
        });
    };

    setActiveClass();

    sidebarItems.forEach(item => {
        item.addEventListener('click', function (e) {
            sidebarItems.forEach(item => {
                item.parentElement.classList.remove('active');
            });

            this.parentElement.classList.add('active');
        });
    });
});