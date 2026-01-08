document.addEventListener("DOMContentLoaded", () => {
  const estado = document.getElementById("Estado");
  const cidade = document.getElementById("Cidade");

  if (!estado || !cidade) return;

  estado.addEventListener("change", async () => {
    const uf = estado.value?.trim();
    cidade.innerHTML = "<option>Carregando...</option>";
    cidade.disabled = true;

    if (!uf) {
      cidade.innerHTML = '<option value="">--Selecione o estado--</option>';
      cidade.disabled = true;
      return;
    }

    try {
      const res = await fetch(`/api/municipios/${encodeURIComponent(uf)}`);
      if (!res.ok) throw new Error("Falha ao obter munic�pios");
      const nomes = await res.json();
      cidade.innerHTML =
        '<option value="">--Selecione a cidade--</option>' +
        nomes
          .map(
            (n) => `<option value="${escapeHtml(n)}">${escapeHtml(n)}</option>`
          )
          .join("");
      cidade.disabled = false;
    } catch (err) {
      console.error(err);
      cidade.innerHTML = '<option value="">Erro ao carregar cidades</option>';
      cidade.disabled = true;
    }
  });

  function escapeHtml(unsafe) {
    return unsafe
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }
});

window.addEventListener("DOMContentLoaded", (event) => {
  const estadoSelect = document.getElementById("Estado");
  const cidadeSelect = document.getElementById("Cidade");

  estadoSelect.addEventListener("change", function () {
    const uf = this.value;

    if (!uf) {
      cidadeSelect.innerHTML =
        '<option value="">--Selecione o estado--</option>';
      cidadeSelect.disabled = true;
      return;
    }

    fetch("/api/municipios/" + encodeURIComponent(uf))
      .then((response) => {
        if (!response.ok) {
          throw new Error("Network response was not ok " + response.statusText);
        }
        return response.json();
      })
      .then((data) => {
        const options = data.map(
          (cidade) =>
            `<option value="${escapeHtml(cidade)}">${escapeHtml(
              cidade
            )}</option>`
        );
        cidadeSelect.innerHTML =
          '<option value="">--Selecione a cidade--</option>' + options.join("");
        cidadeSelect.disabled = false;
      })
      .catch((error) => {
        console.error(
          "There has been a problem with your fetch operation:",
          error
        );
        cidadeSelect.innerHTML =
          '<option value="">Erro ao carregar cidades</option>';
        cidadeSelect.disabled = true;
      });
  });

  function escapeHtml(unsafe) {
    return unsafe
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }
});

(function () {
  const init = () => {
    const $estado = $("#Estado");
    const $cidade = $("#Cidade");

    var $estadoHidden = $('<input type="hidden" name="Estado_value" id="Estado_value" />');
    $estado.after($estadoHidden);
    function syncEstadoHidden() {
      $estadoHidden.val($estado.val() || "");
    }
    $estado.on("change", syncEstadoHidden);
    syncEstadoHidden();

    if (!$estado.length || !$cidade.length || !$.fn.select2) return;

    $cidade.select2({
      placeholder: "--Selecione a cidade--",
      allowClear: true,
      width: "100%",
    });

    var $cidadeHidden = $(
      '<input type="hidden" name="Cidade_value" id="Cidade_value" />'
    );
    $cidade.after($cidadeHidden);
    function syncCidadeHidden() {
      $cidadeHidden.val($cidade.val() || "");
    }
    $cidade.on("change", syncCidadeHidden);
    syncCidadeHidden();

    $cidade.closest("form").on("submit", function () {
      $cidade.prop("disabled", false);
      syncCidadeHidden();
    });

    async function loadCities(uf) {
      if (!uf) {
        $cidade
          .empty()
          .append(new Option("--Selecione o estado--", ""))
          .prop("disabled", true)
          .val(null)
          .trigger("change");
        return;
      }

      $cidade.prop("disabled", true);
      $cidade.empty().append(new Option("Carregando...", "")).trigger("change");

      try {
        const res = await fetch("/api/municipios/" + encodeURIComponent(uf));
        if (!res.ok)
          throw new Error("Falha ao obter munic�pios: " + res.status);
        const nomes = await res.json();
        const data = nomes.map((n) => ({ id: n, text: n }));
        $cidade.empty();
        data.forEach((d) => {
          const option = new Option(d.text, d.id, false, false);
          $cidade.append(option);
        });
        $cidade.prop("disabled", false).val(null).trigger("change");
      } catch (err) {
        console.error(err);
        $cidade
          .empty()
          .append(new Option("Erro ao carregar cidades", ""))
          .prop("disabled", true)
          .val(null)
          .trigger("change");
      }
    }

    $estado.on("change", function () {
      const uf = $(this).val();
      loadCities(uf);
    });

    const initialUf = $estado.val();
    if (initialUf) loadCities(initialUf);
  };

  function tryInit() {
    if (window.jQuery && window.jQuery.fn && window.jQuery.fn.select2) {
      init();
    } else {
      setTimeout(tryInit, 150);
    }
  }

  document.addEventListener("DOMContentLoaded", tryInit);
})();
