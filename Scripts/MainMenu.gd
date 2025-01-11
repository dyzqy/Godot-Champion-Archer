extends Control

func _play():
	get_tree().change_scene_to_file("res://Scenes/Game.tscn")


func _quit():
	get_tree().quit()
