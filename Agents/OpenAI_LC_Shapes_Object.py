import sys
import re
from langchain.llms import OpenAI
from langchain.prompts import PromptTemplate
from langchain.chains import LLMChain

# Set the OpenAI API key
def get_api_key_from_asset(file_path):
    # Read the .asset file
    with open(file_path, 'r') as file:
        content = file.read()
    
    # Regex to find the apiKey line
    match = re.search(r'apiKey: \'([^\']+)\'', content)
    if match:
        return match.group(1).strip()
    else:
        raise ValueError("API key not found in .asset file")

# Specify the path to the AISettings.asset file
settings_file_path = 'UserSettings/AISettings.asset'
api_key = get_api_key_from_asset(settings_file_path)

# Initialize the OpenAI model
llm= OpenAI(model_name='gpt-4', temperature=0.3, max_tokens=2000, api_key=api_key)


# Define the scene template
scene_template = """
Using Unity, please provide a step-by-step guide on creating only the {object_description} using cubes and spheres as 3D objects. Please suggest the overall  size, dimensions of the scene's elements, and the arrangement of the elements. The guide should also be as technical and detailed as possible, without the need for Unity Editor Scripts. 

In your guide, please ensure that all steps are formulated as follows: 

1. Clearly state the action to be performed (e.g., "Create a pathway using cubes"). 
2. Provide specific instructions on where to click and what to select (e.g., "Right-click in the Hierarchy, select 3D Object > Cube").
3. Include steps for renaming and tagging the element (e.g., "Rename and tag the Cube to 'Pathway1'"). 
4. Specify the values for the element's scale, and color (e.g., "Set the and Scale to (1, 0.1, 1) for the first pathway segment"). 
5. Specify the values for the element's transform position, please keep in mind abiding the laws of gravity (e.g., "Set the Transform Position to (0, 0.1, 0)") 
6. Provide a substep for elements written in plural that need to be duplicated (e.g., "Duplicate 'Pathway1' in the Hierarchy to create more pathway segments, adjusting their Transform Positions to create a continuous pathway through the garden"). 
7. Ensure that all shapes to create a certain element are attached to each other. 
8. Avoid overlapping of different elements. 
9. Do not include subtitles within each step.
10. Do not place duplicate items in a straight line. 
11. Make sure that all elements are attached to the ground and are not flying around.
12. The response should not require the need for Unity Editor Scripts or any specific functionality, interactivity, or navigation options. 
13. When generating the element, make sure to stick to this object description: {object_description}.
14. Color the object by changing the value of the sharedMaterial's color of its MeshRenderer component.


Please follow this format for the step in your guide to ensure clarity and consistency.

After generating the step, format it like this code: 
@"Create the ground (grass):
a. Right-click in the Hierarchy, select 3D Object > Cube.
b. Rename and tag the Cube to ""Ground"".
c. Set the Transform Position to (0, 0, 0) and Scale to (10, 0.1, 10).
d. In the Inspector window, click on the Cube's Mesh Renderer component, then click on the color box next to the Albedo in the material settings. Set the color to a suitable grass color (e.g., RGB: 0, 200, 0)."

"""


# Create a prompt template
prompt_temp = PromptTemplate(input_variables=["object_description"], template=scene_template)

# Define the scene description
if len(sys.argv) > 1:
    description = sys.argv[1]
else:
    description = "a blue ball"

# Format the prompt with the scene description
formatted_prompt = prompt_temp.format(object_description=description)

# Initialize the language model chain
chain = LLMChain(llm=llm, prompt=prompt_temp)

#  Generate the guide
generated_guide = chain.run(description)

# Print each step
print(generated_guide)