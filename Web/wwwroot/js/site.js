// JS Dependencies: Popper, Bootstrap & JQuery
import '@popperjs/core';
import 'bootstrap';
import 'jquery';
// Using the next two lines is like including partial view _ValidationScriptsPartial.cshtml
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

// CSS Dependencies: Bootstrap & Bootstrap icons
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'bootstrap/dist/css/bootstrap.css';

// Custom JS imports
import '../../Views/Shared/Components/BookSearch/Default.cshtml.js'

// Custom CSS imports
import '../css/site.css';
import '../../Views/Shared/_Layout.cshtml.css'
import '../../Views/Book/Book.cshtml.css'
import '../../Views/Shared/Components/BookSearch/Default.cshtml.css'

export function redirect(url) {
    window.location.href = url;
}
