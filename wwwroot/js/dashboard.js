//[Dashboard Javascript]

//Project:	Lion Admin - Responsive Admin Template
//Primary use:   Used only for the main dashboard (index.html)


$(function () {

  'use strict';

  // Make the dashboard widgets sortable Using jquery UI
  $('.connectedSortable').sortable({
    placeholder         : 'sort-highlight',
    connectWith         : '.connectedSortable',
    handle              : '.box-header, .nav-tabs',
    forcePlaceholderSize: true,
    zIndex              : 999999
  });
  $('.connectedSortable .box-header, .connectedSortable .nav-tabs-custom').css('cursor', 'move');


 // Morris-chart
	
	if($('#morris_bar_chart').length > 0)
		// Morris bar chart
		Morris.Bar({
			element: 'morris_bar_chart',
			data: [{
				y: '2018',
				a: 100,
				b: 90,
				c: 60,
				f: 40
			}],
			xkey: 'y',
			ykeys: ['a', 'b', 'c', 'f'],
			labels: ['A', 'B', 'C', 'F'],
			barColors:['#ef5350', '#e9ab2e', '#398bf7','#00c292'],
			barOpacity: 0.6,
			hideHover: 'auto',
			grid: false,
			resize: true,
			barGap:7,
			gridTextColor:'#878787',
			gridTextFamily:"Poppins"
		});
	
 // chartjs	
	if( $('#chart_task').length > 0 ){
		var ctx6 = document.getElementById("chart_task").getContext("2d");
		var data6 = {
			 labels: [
			"Completed",
			"Delayed",
			"Overdue"
		],
		datasets: [
			{
				data: [200, 90, 150,90],
				backgroundColor: [
					"#fec107",
					"#00c292",
					"#03a9f3",
				],
				hoverBackgroundColor: [
					"rgba(254, 193, 7, 0.6)",
					"rgba(0, 194, 146, 0.6)",
					"rgba(3, 169, 243, 0.6)",
				]
			}]
		};
		
		var pieChart  = new Chart(ctx6,{
			type: 'pie',
			data: data6,
			options: {
				animation: {
					duration:	3000
				},
				responsive: true,
				maintainAspectRatio:false,
				legend: {
					labels: {
					fontFamily: "Poppins",
					fontColor:"#878787"
					}
				},
				tooltip: {
					backgroundColor:'rgba(33,33,33,1)',
					cornerRadius:0,
					footerFontFamily:"'Poppins'"
				},
				elements: {
					arc: {
						borderWidth: 0
					}
				}
			}
		});
	}
	
	if( $('#chart_risk').length > 0 ){
		var ctx7 = document.getElementById("chart_risk").getContext("2d");
		var data7 = {
			 labels: [
			"Low",
			"Medium",
			"High"
		],
		datasets: [
			{
				data: [300, 500, 50],
				backgroundColor: [
					"#ab8ce4",
					"#fb9678",
					"#03a9f3"
				],
				hoverBackgroundColor: [
					"rgba(171, 140, 228, 0.6)",
					"rgba(251, 150, 120, 0.6)",
					"rgba(3, 169, 243, 0.6)"
				]
			}]
		};
		
		var doughnutChart = new Chart(ctx7, {
			type: 'doughnut',
			data: data7,
			options: {
				animation: {
					duration:	2000
				},
				responsive: true,
				maintainAspectRatio:false,
				legend: {
					labels: {
					fontFamily: "Poppins",
					fontColor:"#878787"
					}
				},
				elements: {
					arc: {
						borderWidth: 0
					}
				},
				tooltip: {
					backgroundColor:'rgba(33,33,33,1)',
					cornerRadius:0,
					footerFontFamily:"'Poppins'"
				}
			}
		});
	}	
	
	
	
	if( $('#ct_chart_2').length > 0 ){
		new Chartist.Line('#ct_chart_2', {
		  labels: [1, 2, 3, 4, 5],
		  series: [
			[1, 5, 10, 0, 1],
			[10, 15, 0, 1, 2]
		  ]
		}, {
		  // Remove this configuration to see that chart rendered with cardinal spline interpolation
		  // Sometimes, on large jumps in data values, it's better to use simple smoothing.
		  lineSmooth: Chartist.Interpolation.simple({
			divisor: 2
		  }),
		  fullWidth: true,
		  chartPadding: {
			right: 20
		  },
		  low: 0
		});
	}
	

	
	

}); // End of use strict
