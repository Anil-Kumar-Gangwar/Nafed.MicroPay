/**
 * Theme: Hyper - Responsive Bootstrap 4 Admin Dashboard
 * Author: Coderthemes
 * Module/App: Form Wizard
 */

$(document).ready(function() {
  $("#basicwizard").bootstrapWizard();

  $("#progressbarwizard").bootstrapWizard({
    onTabShow: function(tab, navigation, index) {
      var $total = navigation.find("li").length;
      var $current = index + 1;
      var $percent = ($current / $total) * 100;
      $("#progressbarwizard")
        .find(".bar")
        .css({ width: $percent + "%" });
    }
  });

  $("#btnwizard").bootstrapWizard({
    nextSelector: ".button-next",
    previousSelector: ".button-previous",
    firstSelector: ".button-first",
    lastSelector: ".button-last"
  });

  $("#rootwizard").bootstrapWizard({
    onNext: function(tab, navigation, index) {
      var $valid = $("#commentForm").valid();
      if (!$valid) {
        $validator.focusInvalid();
        return false;
      }
    }
  });
});
