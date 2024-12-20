let activeExperiment;

window.createExperiment = async function createExperiment() {

    alert("it works.");
    return;
    const form = document.getElementById('inputParameters');
    const formData = new FormData(form);
    const params = {
        participants: Number(formData.get('participants')),
        rounds: Number(formData.get('rounds')),
        venues: Number(formData.get('venues')),
        population: Number(formData.get('population')),
        survivors: Number(formData.get('survivors')),
        mutation: Number(formData.get('mutation')),
    };
    const response = await fetch('/experiments', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(params),
    });
    const result = await response.json();
    activeExperiment = result
    document.getElementById('participants').value = `11`;
    document.getElementById('info').value = `Epoch: ${params.participants}, Metric1-opponents: ${params.rounds},  Metric2-opponents: ${params.venues}`;
};

window.nextGeneration = async function nextGeneration() {

    alert("it works.");
    return;
    const response = await fetch(`/experiments/${activeExperiment}`, {
        method: 'POST',
    });

    if (response.ok) {
        const result = await response.json();

    } else {
        const error = await response.text();
    }
};