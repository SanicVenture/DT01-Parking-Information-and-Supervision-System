# cmake files support debug production
include("${CMAKE_CURRENT_LIST_DIR}/rule.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/file.cmake")

set(DT01_Controller_default_library_list )


# Main target for this project
add_executable(DT01_Controller_default_image_bO5Lzlpp ${DT01_Controller_default_library_list})

set_target_properties(DT01_Controller_default_image_bO5Lzlpp PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${DT01_Controller_default_output_dir})
set_target_properties(DT01_Controller_default_image_bO5Lzlpp PROPERTIES OUTPUT_NAME "default")
set_target_properties(DT01_Controller_default_image_bO5Lzlpp PROPERTIES SUFFIX ".elf")




# Add the link options from the rule file.
DT01_Controller_default_link_rule(DT01_Controller_default_image_bO5Lzlpp)



