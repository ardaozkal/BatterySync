<?php
$file = $_GET["devicename"] . '.txt';

if ($_GET["content"] != null)
{
	$current = $_GET["content"];
}
else
{
	$current = "nothing";
}

	file_put_contents($file, $current);
	echo("successfully set");
?>