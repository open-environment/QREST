$(function () {

    // popover
    $("[data-toggle=popover]").popover();
    $(document).on('click', '.popover-title .close', function (e) {
        var $target = $(e.target), $popover = $target.closest('.popover').prev();
        $popover && $popover.popover('hide');
    });

    // dropdown menu
    $.fn.dropdown.Constructor.prototype.change = function (e) {
        e.preventDefault();
        var $item = $(e.target), $select, $checked = false, $menu, $label;
        !$item.is('a') && ($item = $item.closest('a'));
        $menu = $item.closest('.dropdown-menu');
        $label = $menu.parent().find('.dropdown-label');
        $labelHolder = $label.text();
        $select = $item.find('input');
        $checked = $select.is(':checked');
        if ($select.is(':disabled')) return;
        if ($select.attr('type') == 'radio' && $checked) return;
        if ($select.attr('type') == 'radio') $menu.find('li').removeClass('active');
        $item.parent().removeClass('active');
        !$checked && $item.parent().addClass('active');
        $select.prop("checked", !$select.prop("checked"));

        $items = $menu.find('li > a > input:checked');
        if ($items.length) {
            $text = [];
            $items.each(function () {
                var $str = $(this).parent().text();
                $str && $text.push($.trim($str));
            });

            $text = $text.length < 4 ? $text.join(', ') : $text.length + ' selected';
            $label.html($text);
        } else {
            $label.html($label.data('placeholder'));
        }
    };
    $(document).on('click.dropdown-menu', '.dropdown-select > li > a', $.fn.dropdown.Constructor.prototype.change);

    // tooltip
    $("[data-toggle=tooltip]").tooltip();

    // class
    $(document).on('click', '[data-toggle^="class"]', function (e) {
        e && e.preventDefault();
        var $this = $(e.target), $class, $target, $tmp, $classes, $targets;
        !$this.data('toggle') && ($this = $this.closest('[data-toggle^="class"]'));
        $class = $this.data()['toggle'];
        $target = $this.data('target') || $this.attr('href');
        $class && ($tmp = $class.split(':')[1]) && ($classes = $tmp.split(','));
        $target && ($targets = $target.split(','));
        $targets && $targets.length && $.each($targets, function (index, value) {
            ($targets[index] != '#') && $($targets[index]).toggleClass($classes[index]);
        });
        $this.toggleClass('active');
    });

    // panel toggle
    $(document).on('click', '.panel-toggle', function (e) {
        e && e.preventDefault();
        var $this = $(e.target), $class = 'collapse', $target;
        if (!$this.is('a')) $this = $this.closest('a');
        $target = $this.closest('.panel');
        $target.find('.panel-body').toggleClass($class);
        $this.toggleClass('active');
    });

    // button loading
    //$(document).on('click.button.data-api', '[data-loading-text]', function (e) {
    //    var $this = $(e.target);
    //    $this.is('i') && ($this = $this.parent());
    //    $this.button('loading');
    //});


    // collapse left menu group headers
    $(document).on('click', '.nav-primary a', function (e) {
        var $this = $(e.target), $active;
        $this.is('a') || ($this = $this.closest('a'));
        if ($('.nav-vertical').length) {
            return;
        }
        $active = $this.parent().siblings(".active");
        $active && $active.find('> a').toggleClass('active') && $active.toggleClass('active').find('> ul:visible').slideUp(200);
        ($this.hasClass('active') && $this.next().slideUp(200)) || $this.next().slideDown(200);
        $this.toggleClass('active').parent().toggleClass('active');
        $this.next().is('ul') && e.preventDefault();
        setTimeout(function () { $(document).trigger('updateNav'); }, 300);
    });

    // carousel
    $('.carousel.auto').carousel();

    // dropdown still
    $(document).on('click.bs.dropdown.data-api', '.dropdown .on, .dropup .on', function (e) { e.stopPropagation(); });


    //left menu: make group active based on child active
    $('li.leftmenuhead:has(li.active)').addClass('active');


    //prevent form to be posted multiple times
    $('#norepostform').submit(function () {
        if ($(this).valid()) {
            $(this).find(':submit').attr('disabled', 'disabled');
        }
    });

    //modal: make draggable
    $(".modal-header").on("mousedown", function (mousedownEvt) {
        var $draggable = $(this);
        var x = mousedownEvt.pageX - $draggable.offset().left,
            y = mousedownEvt.pageY - $draggable.offset().top;
        $("body").on("mousemove.draggable", function (mousemoveEvt) {
            $draggable.closest(".modal-dialog").offset({
                "left": mousemoveEvt.pageX - x,
                "top": mousemoveEvt.pageY - y
            });
        });
        $("body").one("mouseup", function () {
            $("body").off("mousemove.draggable");
        });
        $draggable.closest(".modal").one("bs.modal.hide", function () {
            $("body").off("mousemove.draggable");
        });
    });

    //modal: set focus to first visible, enabled, not readonly textbox 
    $(document).on('shown.bs.modal', function (e) {
        $('input:visible:enabled:not([readonly]):first', e.target).focus();
    });


});
