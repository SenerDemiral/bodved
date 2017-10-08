/*jslint white: true, browser: true, undef: true, nomen: true, eqeqeq: true, plusplus: false, bitwise: true, regexp: true, strict: true, newcap: true, immed: true, maxerr: 14 */
/*global window: false, REDIPS: true */

/* enable strict mode */
/*"use strict";*/

// define redipsInit variable
var redipsInit;

// redips initialization
redipsInit = function () {
    console.log("redipsInit");
    var rd = REDIPS.drag;	// reference to the REDIPS.drag class
    rd.dropMode = "switch";
	// DIV container initialization
	rd.init('dragS');
	rd.init('dragD');
	// elements could be cloned with pressed SHIFT key (for demo)
	// rd.clone_shiftKey = true;
	
	rd.event.dropped = function () {
		var pos = rd.getPosition();		// ti, ri, ci, oti, ori, oci
		//console.log('droped', pos, rd.obj.dataset.idx, rd);
		rd.obj.dataset.idx = pos[1];
		rd.objOld.dataset.idx = pos[4];
		
		/*
		var p = rd.obj.innerHTML.indexOf("#");
		if (p == -1)
			rd.obj.innerHTML += ' #' + pos[1];
		else
			rd.obj.innerHTML = rd.obj.innerHTML.substring(0, p) + '#' + pos[1];
			
		p = rd.objOld.innerHTML.indexOf("#");
		if (p == -1)
			rd.objOld.innerHTML += ' #' + pos[4];
		else
			rd.objOld.innerHTML = rd.objOld.innerHTML.substring(0, p) + '#' + pos[4];
		*/
	};

};

// add onload event listener
if (window.addEventListener) {
	window.addEventListener('load', redipsInit, false);
	console.log("addEventListener");
}
/*
else if (window.attachEvent) {
	window.attachEvent('onload', redipsInit);
}*/

