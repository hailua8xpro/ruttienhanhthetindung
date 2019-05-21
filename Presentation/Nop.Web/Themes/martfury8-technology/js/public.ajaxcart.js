/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    usepopupnotifications: false,
    topcartselector: '',
    topwishlistselector: '',
    flyoutcartselector: '',

    init: function (usepopupnotifications, topcartselector, topwishlistselector, flyoutcartselector) {
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            data: $(formselector).serialize(),
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },
    addproducttocart_quickview: function (urladd, formselector) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        $.ajax({
            cache: false,
            url: urladd,
            data: $(formselector).serialize(),
            type: 'post',
            success: this.quickview_success_process,
            complete: function() {
                $('#quickviewmodal .tocart.button.mainbgcolor').removeClass('disable-button');
            },
            error: this.ajaxFailure
        });
    },
    //add a product to compare list
    addproducttocomparelist: function (urladd) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            cache: false,
            url: urladd,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },
    quickview_success_process: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
           
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success == true) {
                if (response.updatetopwishlistsectionhtml) {
                    $('.towishlist').data('toggle', 'tooltip').data('placement', 'top').attr('title', 'Thêm vào yêu thích thành công');
                    $('.towishlist').tooltip('show');
                }
                else if (response.updatetopcartsectionhtml) {
                    $.notify({
                        icon: 'mbi mbi-check',
                        message: response.succsessnotify,
                        url: response.carturl
                    },
                        {
                            type: 'success',
                            z_index:1500
                        });
                }
                else {
                    $('.tocompare').data('toggle', 'tooltip').data('placement', 'top').attr('title', 'Thêm vào so sánh thành công');
                    $('.tocompare').tooltip('show');
                }
                
            }
            else {
                $.notify({
                    icon: 'mbi mbi-check',
                    message: response.message,
                    url: response.carturl
                },
                    {
                        type: 'danger',
                        z_index: 1500
                    });
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },
    success_process: function (response) {
        $('.product-btn a.disabled').removeClass('disabled');
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success == true) {
                //success
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    //displayBarNotification(response.message, 'success', 3500);
                    $.notify({
                        icon:'mbi mbi-check',
                        message: response.succsessnotify,
                        url: response.carturl
                    },
                    {
                        type:'success'
                    });
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    $.notify({
                        message: response.message,
                        url: response.carturl
                    },
                        {
                            type: 'danger'
                        });

                }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
        AjaxCart.setLoadWaiting(false);
    },

    ajaxFailure: function () {
        alert('Failed to add the product. Please refresh the page and try one more time.');
    }
};