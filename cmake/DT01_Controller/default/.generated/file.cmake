# The following variables contains the files used by the different stages of the build process.
set(DT01_Controller_default_image_name "default.elf")
set(DT01_Controller_default_image_base_name "default")

# The output directory of the final image.
set(DT01_Controller_default_output_dir "${CMAKE_CURRENT_SOURCE_DIR}/../../../out/DT01_Controller")

# The full path to the final image.
set(DT01_Controller_default_full_path_to_image ${DT01_Controller_default_output_dir}/${DT01_Controller_default_image_name})
