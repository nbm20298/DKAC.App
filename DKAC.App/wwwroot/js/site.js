(function () {
    var sitePlusMinus = function () {

        var value, quantity = document.getElementsByClassName('quantity-container');

        function createBindings(quantityContainer) {
            var quantityAmount = quantityContainer.getElementsByClassName('quantity-amount')[0];
            var increase = quantityContainer.getElementsByClassName('increase')[0];
            var decrease = quantityContainer.getElementsByClassName('decrease')[0];
            increase.addEventListener('click', function (e) { increaseValue(e, quantityAmount); });
            decrease.addEventListener('click', function (e) { decreaseValue(e, quantityAmount); });
        }

        function init() {
            for (var i = 0; i < quantity.length; i++) {
                createBindings(quantity[i]);
            }
        };

        function increaseValue(event, quantityAmount) {
            value = parseInt(quantityAmount.value, 10);

            value = isNaN(value) ? 0 : value;
            value++;
            quantityAmount.value = value;
        }

        function decreaseValue(event, quantityAmount) {
            value = parseInt(quantityAmount.value, 10);

            value = isNaN(value) ? 0 : value;
            if (value > 0) value--;
            quantityAmount.value = value;
        }

        init();
    };
    sitePlusMinus();
})();

// isotope js
$(window).on('load', function () {
    $('.filters_menu li').click(function () {
        $('.filters_menu li').removeClass('active-menu');
        $(this).addClass('active-menu');

        var data = $(this).attr('data-filter');
        $grid.isotope({
            filter: data
        })
    });

    var $grid = $(".grid").isotope({
        itemSelector: ".all",
        percentPosition: false,
        masonry: {
            columnWidth: ".all"
        }
    })
});

// Show Menu, Login
const login = document.querySelector('.login-form');
const navbar = document.getElementById("nav");
const logged = document.getElementById("logged_form");

$(document).ready(function () {
    $("#login").click(function () {
        login.classList.toggle('active');
    });
    $("#logged").click(function () {
        logged.classList.toggle('active');
        login.classList.remove('active');
    });
});

window.onscroll = () => {
    navbar.classList.remove('active');
    login.classList.remove('active');
}

var swiper = new Swiper(".home-slider", {
    autoplay: {
        delay: 7500,
        disableOnInteraction: false,
    },
    grabCursor: true,
    loop: true,
    centeredSlides: true,
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
});


var swiper = new Swiper(".menu-slider", {
    grabCursor: true,
    loop: true,
    autoHeight: true,
    centeredSlides: true,
    spaceBetwwen: 20,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
});


//click checkbox show quantity và giá trị = 1 của đăng ký 1 user
function toggleQuantityInput(checkbox, shiftInputId, quantityInputId, shiftID) {
    var shiftInput = document.getElementById(shiftInputId);
    var quantityInput = document.getElementById(quantityInputId);

    if (checkbox.checked) {
        shiftInput.value = shiftID;
        quantityInput.disabled = false;
        if (quantityInput.value === '') {
            quantityInput.value = 1;
        }
    } else {
        shiftInput.value = '';
        quantityInput.disabled = true;
        quantityInput.value = '';

    }
}

//Click checkbox show quantity và đăng kí nhiều user
function toggleQuantityInputs(checkbox, shiftInputId, quantityInputId, userIDInputId, userID, shiftID) {
    var shiftInput = document.getElementById(shiftInputId);
    var quantityInput = document.getElementById(quantityInputId);
    var userIDInput = document.getElementById(userIDInputId);

    if (checkbox.checked) {
        shiftInput.value = shiftID;
        userIDInput.value = userID;
        quantityInput.disabled = false;
        if (quantityInput.value === '') {
            quantityInput.value = '1';
        }
    } else {
        shiftInput.value = '';
        quantityInput.disabled = true;
        quantityInput.value = '';
        userIDInput.value = '';
    }
}

function toggleQuantityUpdate(checkbox, shiftInputId, quantityInputId, userIDInputId) {
    var shiftInput = document.getElementById(shiftInputId);
    var quantityInput = document.getElementById(quantityInputId);
    var userIDInput = document.getElementById(userIDInputId);

    if (checkbox.checked) {
        quantityInput.disabled = false;
        if (quantityInput.value === '') {
            quantityInput.value = '1';
        }
        shiftInput.value = checkbox.getAttribute('data-shiftid');
        userIDInput.value = checkbox.getAttribute('data-userid');
    } else {
        shiftInput.value = '';
        quantityInput.disabled = true;
        quantityInput.value = '';
        userIDInput.value = '';
    }
}

//check day input register
document.addEventListener("DOMContentLoaded", function () {
    var orderDateInput = document.getElementById('orderDate');
    var now = new Date();

    // Định dạng ngày giờ thành chuỗi "yyyy-MM-ddTHH:mm"
    function formatDateTime(date) {
        var day = ("0" + date.getDate()).slice(-2);
        var month = ("0" + (date.getMonth() + 1)).slice(-2);
        var year = date.getFullYear();
        var hours = ("0" + date.getHours()).slice(-2);
        var minutes = ("0" + date.getMinutes()).slice(-2);

        return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
    }

    // Đặt giá trị mặc định là ngày giờ hiện tại nếu không có giá trị
    if (!orderDateInput.value) {
        orderDateInput.value = formatDateTime(now);
    }
})

