
// Gets all of the canvas element in the layout page
var c1 = document.getElementById("myCanvas1");
var c2 = document.getElementById("myCanvas2");
var c3 = document.getElementById("myCanvas3");

// Draw a star element on each of the canvas element
var ctx1 = c1.getContext("2d");
star(ctx1, 25, 25, 10, 5, 0.4)

var ctx2 = c2.getContext("2d");
star(ctx2, 25, 25, 10, 5, 0.4)

var ctx3 = c3.getContext("2d");
star(ctx3, 25, 25, 10, 5, 0.4)

// Draw star functions
function star(ctx, x, y, r, p, m)
{
    ctx.save();
    ctx.beginPath();
    ctx.translate(x, y);
    ctx.moveTo(0, 0 - r);

    for (var i = 0; i < p; i++)
    {
        ctx.rotate(Math.PI / p);
        ctx.lineTo(0, 0 - (r * m));
        ctx.rotate(Math.PI / p);
        ctx.lineTo(0, 0 - r);
    }

    ctx.fillStyle = "#e6e6e6";
    ctx.fill();
    ctx.restore();
}