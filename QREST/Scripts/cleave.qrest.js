$(document).ready(function () {
    //numbers only (with comma)
    $('.cleave-num').toArray().forEach(function (field) {
        new Cleave(field, {
            numeral: true
        });
    });

    //numbers only (with comma)
    $('.cleave-num-no-comma').toArray().forEach(function (field) {
        new Cleave(field, {
            numericOnly: true,
            blocks: [4]
        });
    });

    //phone entry
    $('.phone-10').toArray().forEach(function (field) {
        new Cleave(field, {
            numericOnly: true,
            blocks: [0, 3, 0, 3, 4],
            delimiters: ["(",")", " ", "-"]
        });
    });


    //date entry
    $('.date-cleave').toArray().forEach(function (field) {
        new Cleave(field, {
            date: true,
            datePattern: ['m','d','Y']
        });
    });


    //time entry
    $('.time-cleave').toArray().forEach(function (field) {
        new Cleave(field, {
            time: true,
            timePattern: ['h', 'm']
        });
    });


    //zip entry
    $('.zip-cleave').toArray().forEach(function (field) {
        new Cleave(field, {
            numericOnly: true,
            blocks: [5, 4],
            delimiter: '-'
        });
    });
});