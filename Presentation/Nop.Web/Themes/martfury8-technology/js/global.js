
$(document).ready(function () {
    $('.btn-search-mobile .mbi-magnifier').magnificPopup({
        items: {
            type: 'inline',
            src: '.col-search'
        },
        mainClass: 'search-popup'
    });
    $('.btn-nav').click(function () {
        $(this).addClass('active');
        $('html').addClass('nav-open');
        $('.magebig-container').addClass('overlay-open');
        $('.magebig-mobile-menu').addClass('open');
    })
    $('.close-nav').click(function () {
        $('html').removeClass('nav-open');
        $('.magebig-container').removeClass('overlay-open');
        $('.magebig-mobile-menu').removeClass('open');
        $('.btn-nav').removeClass('active');
    })
    $('.dropdown-menu').click(function (e) {
        e.stopPropagation();
    });
    $(document).on('click', 'ul.nav-collapse li .mbi-ios-arrow-down', function () {
        $(this).removeClass('mbi-ios-arrow-down');
        $(this).addClass('mbi-ios-arrow-up');
        $(this).closest('li').find('ul').slideToggle();
        return false;
    })
    $(document).on('click', 'ul.nav-collapse li .mbi-ios-arrow-up', function () {
        $(this).removeClass('mbi-ios-arrow-up');
        $(this).addClass('mbi-ios-arrow-down');
        $(this).closest('li').find('ul').slideToggle();
        return false;
    })
    $('.lazy').Lazy();

    $('.sidebar-main .toggle-class').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })
    $('.sidebar-main .close-expanded').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })
    $('.sidebar-main .close-expand-mb').click(function () {
        $('body').toggleClass('hide-over');
        var block = $(this).closest('.block');
        $(block).toggleClass('active');
        $(block).find('.block-content').toggleClass('show-expanded');
    })


})
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})
function showQuickViewProduct(url) {
    if ($('#quickviewmodal').length>0) {
        $('#quickviewmodal').remove();

    }
    $.ajax({
        type: 'get',
        url: url,
        beforeSend: function () {
            displayAjaxLoading(true);
        },
        success: function (e) {
            displayAjaxLoading();
            if (e.success === true) {
                $('body').append(e.html);
                $('#quickviewmodal').modal({
                    keyboard: true
                });
                if ($('#quickviewmodal .swiper-container').length > 0) {
                    var productdetailthumbimageslider = new Swiper('.product-detail-gallerythumb-slider', {
                        spaceBetween: 10,
                        slidesPerView: 4,
                        loop: true,
                        freeMode: true,
                        loopedSlides: 5, //looped slides should be the same
                        watchSlidesVisibility: true,
                        watchSlidesProgress: true,
                    });
                    var productdetailimageslider = new Swiper('.product-detail-gallery-slider', {
                        spaceBetween: 10,
                        loop: true,
                        loopedSlides: 5,
                        zoom: true,
                        autoHeight: true,
                        thumbs: {
                            swiper: productdetailthumbimageslider,
                        }
                    });
                }
            }
        }

    })
}


$('#quickviewmodal').on('hidden.bs.modal', function () {
    $('#quickviewmodal').remove();
})