toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "3000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

class Notifier {
    constructor(opt) {
        this.dflt = {
            info: {

            },
            success: {
                "progressBar": true
            },
            warning: {

            },
            error: {

            }

        }
        this.cfg = Object.assign({}, opt, this.dflt);

    }

    info(msg, tl, cfgOvr) {
        this.notify("info", msg, tl, cfgOvr)
    }

    success(msg, tl, cfgOvr) {
        this.notify('success', msg, tl, cfgOvr);
    }

    warning(msg, tl, cfgOvr) {
        this.notify('warning', msg, tl, cfgOvr);
    }

    error(msg, tl, cfgOvr) {
        this.notify('error', msg, tl, cfgOvr);
    }

    notify(lvl, msg, tl, cfgOvr) {
        let cfg = this.cfg[lvl];
        if (cfgOvr) {
            cfg = Object.assign({}, cfgOvr, cfg);
        }
        window.toastr[lvl](msg, tl, cfg)
    }
}