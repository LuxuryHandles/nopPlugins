(function () {

  function bySel(s, root) { return (root || document).querySelector(s); }
  function bySelAll(s, root) { return (root || document).querySelectorAll(s); }

  // Is this a brand-new outcome (no data saved yet)?
  function isNewOutcome() {
    var form = bySel('#outcomeForm');
    if (!form) return false;

    // Prefer data-existing on the form
    var d = (form.getAttribute('data-existing') || '').toLowerCase();
    if (d === 'false' || d === '0' || d === '') return true;
    if (d === 'true' || d === '1') return false;

    // Fallback to hidden input if needed
    var hidden = bySel('input[name="Existing"]');
    if (!hidden) return false;
    var v = (hidden.value || '').toLowerCase();
    return (v === 'false' || v === '0' || v === '');
  }

  // ------------------------
  // Level 1 (category) toggle
  // ------------------------
  function toggleL1() {
    bySelAll('input[data-l1]').forEach(function (cb) {
      var key = cb.getAttribute('data-l1');           // e.g., "mental"
      var sec = document.getElementById('l1-' + key); // e.g., #l1-mental
      if (!sec) return;

      // Show/hide the whole section
      sec.style.display = cb.checked ? 'block' : 'none';

      if (cb.checked) {
        // NEW: if it's a new page (no data yet) and nothing selected under this section,
        // auto-show ALL subcategories (mimics your original first-time UX).
        var anySelected = sec.querySelector('.outcome-block input[type="checkbox"][data-l2]:checked');
        if (!anySelected && isNewOutcome()) {
          setShowHidden(sec, true);   // show all
        }
      } else {
        // When turning a section off, collapse hidden blocks again
        setShowHidden(sec, false);
      }
    });
  }

  // ------------------------------------
  // Survey conditional inside one block
  // ------------------------------------
  function applySurveyToggle(selectEl) {
    var block = selectEl.closest('.outcome-block');
    if (!block) return;
    var v = selectEl.value; // "0"=Pre, "1"=Post, "2"=One-off, ""=placeholder
    var oneoff = block.querySelector('.conditional.oneoff');
    var prepost = block.querySelector('.conditional.prepost');

    if (oneoff) oneoff.style.display = (v === '2') ? 'block' : 'none';
    if (prepost) prepost.style.display = (v === '0' || v === '1') ? 'block' : 'none';
  }

  // -------------------------------
  // Clear all inputs in one block
  // -------------------------------
  function clearBlockFields(block) {
    if (!block) return;
    var content = block.querySelector('.outcome-content');
    if (!content) return;

    content.querySelectorAll('input[type="text"], input[type="number"], textarea').forEach(function (el) {
      el.value = "";
    });

    content.querySelectorAll('select').forEach(function (sel) {
      sel.value = ""; // placeholder option so model binds to null
      if (sel.classList.contains('survey-type')) applySurveyToggle(sel);
    });
  }

  // -----------------------------------------------------
  // Level 2 (outcome) checkbox -> show/hide its content,
  // manage hidden state of the whole block
  // -----------------------------------------------------
  function toggleOutcomeBlock(cb) {
    var block = cb.closest('.outcome-block');
    var section = cb.closest('.category-section');
    if (!block || !section) return;

    var content = block.querySelector('.outcome-content');
    var show = !!cb.checked;

    if (content) content.style.display = show ? 'block' : 'none';

    if (show) {
      block.classList.remove('is-hidden');
    } else {
      clearBlockFields(block);
      block.classList.add('is-hidden'); // CSS hides unless section has .show-hidden
    }
  }

  // ----------------------------------------------------
  // Section-level: Show/Hide hidden outcomes toggle
  // ----------------------------------------------------
  function setShowHidden(section, show) {
    section.classList.toggle('show-hidden', !!show);

    var btn = section.querySelector('.toggle-hidden');
    if (btn) {
      // Preserve the original "Show..." label from markup
      var showText = btn.getAttribute('data-show-text') || btn.dataset.showText || btn.textContent.trim();
      var hideText = btn.getAttribute('data-hide-text') || btn.dataset.hideText || 'Hide';

      // cache for next time
      btn.dataset.showText = showText;
      btn.dataset.hideText = hideText;

      btn.setAttribute('aria-expanded', show ? 'true' : 'false');
      btn.textContent = show ? hideText : showText;
    }
  }

  function setupHiddenToggleForSection(section) {
    var btn = section.querySelector('.toggle-hidden');
    if (!btn) return;

    // seed default labels (from existing text content if attributes are not provided)
    btn.dataset.showText = btn.getAttribute('data-show-text') || btn.textContent.trim();
    btn.dataset.hideText = btn.getAttribute('data-hide-text') || 'Hide';

    btn.addEventListener('click', function () {
      var wantShow = !section.classList.contains('show-hidden');
      setShowHidden(section, wantShow);
    });

    // Default after load: keep hidden blocks collapsed
    setShowHidden(section, false);
  }

  // ------------------------
  // Event wiring
  // ------------------------
  document.addEventListener('change', function (e) {
    if (e.target.matches('input[data-l1]')) toggleL1();
    if (e.target.matches('input[type="checkbox"][data-l2]')) toggleOutcomeBlock(e.target);
    if (e.target.matches('select.survey-type')) applySurveyToggle(e.target);
  });

  // ------------------------
  // Initial state on load
  // ------------------------
  document.addEventListener('DOMContentLoaded', function () {
    // L1 sections
    toggleL1();

    // Per-section show/hidden toolbar
    bySelAll('.category-section').forEach(setupHiddenToggleForSection);

    // Initialize each outcome block from its checkbox state
    bySelAll('.outcome-block input[type="checkbox"][data-l2]').forEach(function (cb) {
      toggleOutcomeBlock(cb);
    });

    // Initialize survey conditionals
    bySelAll('select.survey-type').forEach(function (sel) {
      applySurveyToggle(sel);
    });
  });

})();
